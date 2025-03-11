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

public static class AccountingEndpoints
{

    public static void MapAccountingEndpoints(this IEndpointRouteBuilder routes)
    {
        var loggerFactory = routes.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AccountingEndpoints");
        

        var group = routes.MapGroup("/api/Accounting").WithTags("Accounting");

        //add expense
        group.MapPost("/Revenue", async (Revenue revenue, ShuleOneDatabaseContext db, FileGeneratorService fileGeneratorService) =>
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
                return TypedResults.Created($"/api/Accounting/{revenue}", revenue);
            }
            else
            {
                studentDetails = studentDetailsResult.Value;
            }

            //create receipt record
            Receipt receipt = new Receipt
            {
                fk_student_id = studentDetails.id,
                fk_revenue_id = revenue.id,
            };

            db.Receipts.Add(receipt);
            await db.SaveChangesAsync();

            ReceiptItem receiptItem = new ReceiptItem
            {
                fk_receipt_id = receipt.id,
                amount = revenue.amount,
                item_type = ReceiptItemType.Generic
            };

            db.ReceiptItems.Add(receiptItem);
            await db.SaveChangesAsync();


            //generate receipt
            string studentName = $"{studentDetails.other_names } { studentDetails.surname}";
            string fileName = $"{studentDetails.id} - {studentName}_Receipt-{DateTime.UtcNow.ToString("ddMMyyHHmmss")}.pdf";
            string outputFilePath = $"/GeneratedReports/Receipts/{fileName}";
            string templateFileName = "LifewayReceiptTemplate.docx";

            Object receiptDetails = new
            {
                admnNo = studentDetails.id,
                studentName = studentName,
                totalPaid = revenue.amount,
                paidBy = revenue.paid_by,
                datePaid = revenue.payment_date.ToString("DD/MM/YYYY"),
                receiptItems = db.ReceiptItems.Where(r => r.fk_receipt_id == receipt.id).Select(r => new
                {
                    amount = r.amount,
                    item = r.item_type.ToString()
                }).ToList()
            };

            JObject reportDetails = JObject.FromObject(receiptDetails);
            await fileGeneratorService.GenerateFile(reportDetails, fileName, outputFilePath, templateFileName);

            //notify parent

            return TypedResults.Created($"/api/Finance/{revenue}", revenue);
        })
        .WithName("ProcessRevenueCollection")
        .WithOpenApi();


        //add revenue

        //get expenses

        //get revenues


    }

    private static Result<Student> GetStudentDetails(ShuleOneDatabaseContext db, string accountNumber)
    {
        if(string.IsNullOrEmpty(accountNumber))
        {
            return Result.Failure<Student>("Missing admission number.");
        }

        if(!int.TryParse(accountNumber, out int studentId))
        {
            return Result.Failure<Student>("Invalid admission number");
        }

        var student = db.Student.AsNoTracking().FirstOrDefault(s => s.id == studentId);

        return student == null ? Result.Failure<Student>("Admission number not found.") : Result.Success(student);  
    }
}
