using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.Models;
using Prema.ShuleOne.Web.Server.BulkSms;
using Prema.ShuleOne.Web.Server.Services;
using AutoMapper;
using AutoMapper.Execution;
using Newtonsoft.Json.Linq;
using Prema.ShuleOne.Web.Server.Endpoints.Reports;
using static Prema.ShuleOne.Web.Server.Controllers.FinanceEndpint;
namespace Prema.ShuleOne.Web.Server.Endpoints;
using CSharpFunctionalExtensions;
using global::Telegram.Bot.Types;
using Microsoft.Extensions.Options;
using Prema.ShuleOne.Web.Server.AppSettings;
using System.Security.Cryptography.Pkcs;
using System.Text;

public static class AccountingEndpoints
{

    public static void MapAccountingEndpoints(this IEndpointRouteBuilder routes)
    {
        var loggerFactory = routes.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AccountingEndpoints");


        var group = routes.MapGroup("/api/Accounting").WithTags("Accounting");

        //add expense
        group.MapPost("/Revenue", async (Revenue revenue, ShuleOneDatabaseContext db, string accountName, FileGeneratorService fileGeneratorService) =>
        {
            //save revenue record
            db.Revenue.Add(revenue);
            await db.SaveChangesAsync();

            //check if account number valid
            Student studentDetails = new Student();
            var studentDetailsResult = GetStudentDetails(db, revenue.account_number);

            if (studentDetailsResult.IsFailure)
            {
                logger.LogWarning(studentDetailsResult.Error);
            }
            else
            {
                studentDetails = studentDetailsResult.Value;

                await ProcessReceipt(studentDetails, revenue, db, logger, fileGeneratorService);

                //notify parent via sms
                //already done on budget tracker - to be moved here later


                //create transactions
                //get id of default account
                int defaultAccountId = GetDefaultAccountId(revenue, db);
                if (defaultAccountId == 0)
                {
                    logger.LogWarning("Default account not set.");

                    revenue.status = RevenueStatus.TransactionPending;
                }
                else
                {
                    Transaction transaction = new Transaction()
                    {
                        amount = revenue.amount,
                        description = "School Fee Paid",
                        reference_id = revenue.payment_reference,
                        created_by = "0",
                    };

                    db.Transaction.Add(transaction);
                    await db.SaveChangesAsync();

                    db.JournalEntry.Add(new JournalEntry()
                    {
                        amount = revenue.amount,
                        fk_account_id = defaultAccountId, //main bank
                        fk_transaction_id = transaction.id,
                        type = JournalEntryType.Credit
                    });

                    db.JournalEntry.Add(new JournalEntry()
                    {
                        amount = revenue.amount,
                        fk_account_id = 1, //fees TODO: get account id for fees
                        fk_transaction_id = transaction.id,
                        type = JournalEntryType.Debit
                    });

                    revenue.status = RevenueStatus.Allocated;
                }
            }

            db.Revenue.Update(revenue);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/api/Finance/{revenue}", revenue);
        })
        .WithName("ProcessRevenueCollection")
        .WithOpenApi();

        //download receipt
        group.MapGet("/Receipt/{receiptId}", async (int receiptId, ShuleOneDatabaseContext db, IOptionsMonitor<ReportSettings> reportSettings) =>
        {
            var receipt = db.Receipts.Include(r => r.ReceiptItems).FirstOrDefault(r => r.id == receiptId);
            if (receipt == null)
            {
                logger.LogWarning("Receipt record not found.");
                return TypedResults.NotFound("Receipt record not found.");
            }

            if (!IsFileLocationValid(receipt, reportSettings.CurrentValue.FileStoragePath))
            {
                logger.LogWarning("Receipt file not found.");
                return TypedResults.NotFound("Receipt file not found.");
            }

            var filePath = Path.Combine(reportSettings.CurrentValue.FileStoragePath, receipt.file_location);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return Results.File(fileStream, "application/pdf", Path.GetFileName(receipt.file_location));
        });

        //add revenue

        //get expenses

        //get revenues


    }

    private static int GetDefaultAccountId(Revenue revenue, ShuleOneDatabaseContext db)
    {
        var account = db.Account.FirstOrDefault(a => a.default_source == revenue.payment_method);

        if(account == null)
        {
            return 0;
        }

        return account.id;
    }

    private static async Task ProcessReceipt(Student studentDetails, Revenue revenue, ShuleOneDatabaseContext db, ILogger logger, FileGeneratorService fileGeneratorService)
    {
        //create receipt record
        Receipt receipt = new Receipt
        {
            fk_student_id = studentDetails.id,
            fk_revenue_id = revenue.id,
            status = ReceiptStatus.Valid
        };

        db.Receipts.Add(receipt);
        await db.SaveChangesAsync(); //error here

        ReceiptItem receiptItem = new ReceiptItem
        {
            fk_receipt_id = receipt.id,
            amount = revenue.amount,
            item_type = ReceiptItemType.Generic
        };

        db.ReceiptItems.Add(receiptItem);
        await db.SaveChangesAsync();


        //generate receipt
        var generateReceiptFileResult = GenerateReceiptFile(logger, studentDetails, receipt, revenue, fileGeneratorService, db);


        //update receipt record with file location
        if (generateReceiptFileResult.IsSuccess)
        {
            receipt.file_location = generateReceiptFileResult.Value;
            receipt.file_location_type = FileLocationType.Local;
            db.Receipts.Update(receipt);
            await db.SaveChangesAsync();
        }
    }
    private static Result<Student> GetStudentDetails(ShuleOneDatabaseContext db, string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber))
        {
            return Result.Failure<Student>("Missing admission number.");
        }

        if (!int.TryParse(accountNumber, out int studentId))
        {
            return Result.Failure<Student>("Invalid admission number");
        }

        var student = db.Student.AsNoTracking().FirstOrDefault(s => s.id == studentId);

        return student == null ? Result.Failure<Student>("Admission number not found.") : Result.Success(student);
    }

    public static string GenerateHtmlTable(List<ReceiptItem> items)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<table style='width:30%; border-collapse:collapse;'>");
        sb.AppendLine("<tr><th style='text-align:left;'>Item</th><th style='text-align:left;'>Amount</th></tr>");

        foreach (var item in items)
        {
            sb.AppendLine($"<tr><td>{item.item_type.ToString()}</td><td>{item.amount}</td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    public static bool IsFileLocationValid(Receipt receipt, string baseFileStoragePath)
    {
        switch (receipt.file_location_type)
        {
            case FileLocationType.Local:
                return File.Exists(Path.Combine(baseFileStoragePath, receipt.file_location));
            default:
                return false;
        }
    }

    private static Result<string> GenerateReceiptFile(ILogger logger, Student studentDetails, Receipt receipt, Revenue revenue, FileGeneratorService fileGeneratorService, ShuleOneDatabaseContext db)
    {
        try
        {
            //generate receipt
            string studentName = $"{studentDetails.other_names} {studentDetails.surname}";
            string fileName = $"{studentDetails.id} - {studentName}_Receipt-{receipt.id}.pdf";
            string outputFilePath = Path.Combine("Receipts", fileName);
            string templateFileName = "LifewayReceiptTemplate.docx";

            Object receiptDetails = new
            {
                receiptNo = receipt.id,
                admNo = studentDetails.id,
                studentName = studentName,
                totalPaid = revenue.amount,
                paidBy = revenue.paid_by,
                datePaid = revenue.payment_date.ToString("dd/MM/yyyy"),
                receiptItems = db.ReceiptItems.Where(r => r.fk_receipt_id == receipt.id).Select(r => new
                {
                    amount = r.amount,
                    item = r.item_type.ToString()
                }).ToList()
            };

            JObject reportDetails = JObject.FromObject(receiptDetails);
            var generateFileResult = fileGeneratorService.GenerateFile(reportDetails, fileName, outputFilePath, templateFileName);

            return Result.Success<string>(outputFilePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating receipt.");
            return Result.Failure<string>("Error generating file.");
        }
    }
}
