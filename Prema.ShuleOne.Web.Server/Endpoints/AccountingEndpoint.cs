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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Prema.ShuleOne.Web.Server.AppSettings;
using System.Security.Cryptography.Pkcs;
using System.Security.Principal;
using Microsoft.AspNetCore.Antiforgery;
using System.Text;

public static class AccountingEndpoints
{

    public static void MapAccountingEndpoints(this IEndpointRouteBuilder routes)
    {
        var loggerFactory = routes.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AccountingEndpoints");


        var group = routes.MapGroup("/api/Accounting").WithTags("Accounting");

        //add revenue
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
            }
            else
            {
                studentDetails = studentDetailsResult.Value;

                await ProcessReceipt(studentDetails, revenue, db, logger, fileGeneratorService);

                //notify parent via sms
                //already done on budget tracker - to be moved here later


                //create transactions
                //get id of default account
                Account defaultBankAccount = GetDefaultAccountId(revenue.payment_method, db);
                Account defaultFeeAccount = GetDefaultAccountId(PaymentMethod.InternalTransaction, db);
                if (defaultBankAccount == null || defaultFeeAccount == null)
                {
                    logger.LogWarning("Default accounts not set.");

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
                        transaction_type = TransactionType.FeeReceived,
                        fk_transaction_type_identifier = studentDetails.id,
                    };

                    db.Transaction.Add(transaction);
                    await db.SaveChangesAsync();

                    db.JournalEntry.Add(new JournalEntry()
                    {
                        amount = revenue.amount,
                        fk_account_id = defaultBankAccount.id, //main bank
                        fk_transaction_id = transaction.id,
                        fk_journal_entry_type = (int)JournalEntryType.Debit
                    });

                    db.JournalEntry.Add(new JournalEntry()
                    {
                        amount = revenue.amount,
                        fk_account_id = defaultFeeAccount.id, //fees TODO: get account id for fees
                        fk_transaction_id = transaction.id,
                        fk_journal_entry_type = (int)JournalEntryType.Credit
                    });

                    revenue.status = RevenueStatus.Allocated;
                    revenue.fk_intended_account_number = studentDetails.id;

                    defaultBankAccount.balance += revenue.amount;
                    defaultFeeAccount.balance -= revenue.amount;
                    db.Update(defaultFeeAccount);
                    db.Update(defaultFeeAccount);
                    await db.SaveChangesAsync();
                }
            }

            db.Revenue.Update(revenue);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/api/Finance/{revenue}", revenue);
        })
        .WithName("ProcessRevenueCollection")
        .WithOpenApi();

        //record manual fee payement
        group.MapPost("/Revenue/ManualUpdate", async Task<Results<Created<RevenueWithStudentDto>, NotFound<string>>>
            (int revenueId, int studentId, ShuleOneDatabaseContext db, FileGeneratorService fileGeneratorService) =>
        {

            //check if account number valid
            Student studentDetails = db.Student.AsNoTracking()
                .FirstOrDefault(s => s.id == studentId);

            if (studentDetails == null)
            {
                return TypedResults.NotFound("Student not found.");
            }

            Revenue revenue = db.Revenue.AsNoTracking()
                .FirstOrDefault(r => r.id == revenueId);

            if (revenue == null)
            {
                return TypedResults.NotFound("Revenue not found.");
            }


            await ProcessReceipt(studentDetails, revenue, db, logger, fileGeneratorService);

            //notify parent via sms
            //already done on budget tracker - to be moved here later

            //create transactions
            //get id of default account
            Account defaultBankAccount = GetDefaultAccountId(revenue.payment_method, db);
            Account defaultFeeAccount = GetDefaultAccountId(PaymentMethod.InternalTransaction, db);
            if (defaultBankAccount == null || defaultFeeAccount == null)
            {
                logger.LogWarning("Default accounts not set.");

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
                    transaction_type = TransactionType.FeeReceived,
                    fk_transaction_type_identifier = studentDetails.id,
                };

                db.Transaction.Add(transaction);
                await db.SaveChangesAsync();

                db.JournalEntry.Add(new JournalEntry()
                {
                    amount = revenue.amount,
                    fk_account_id = defaultBankAccount.id, //main bank
                    fk_transaction_id = transaction.id,
                    fk_journal_entry_type = (int)JournalEntryType.Debit
                });

                db.JournalEntry.Add(new JournalEntry()
                {
                    amount = revenue.amount,
                    fk_account_id = defaultFeeAccount.id, //fees TODO: get account id for fees
                    fk_transaction_id = transaction.id,
                    fk_journal_entry_type = (int)JournalEntryType.Credit
                });

                revenue.status = RevenueStatus.Allocated;
                revenue.fk_intended_account_number = studentDetails.id;

                defaultBankAccount.balance += revenue.amount;
                defaultFeeAccount.balance -= revenue.amount;
                db.Update(defaultFeeAccount);
                db.Update(defaultFeeAccount);
                await db.SaveChangesAsync();
            }


            db.Revenue.Update(revenue);
            await db.SaveChangesAsync();


            RevenueWithStudentDto revenueDetails = db.Revenue
                .AsNoTracking()
                .Join(
                    db.Student,
                    revenue => revenue.fk_intended_account_number,
                    student => student.id,
                    (revenue, student) => new RevenueWithStudentDto
                    {
                        Revenue = revenue,
                        Student = student
                    }
                )
                .Where(s => s.Revenue.id == revenue.id)
                .OrderBy(s => s.Revenue.id)
                .FirstOrDefault();

            return TypedResults.Created($"/api/Finance", revenueDetails);
        })
        .WithName("ProcessManualRevenueCollection")
        .WithOpenApi();

        //download receipt
        group.MapGet("/Receipt/{receiptId}", async
            (int revenueId, ShuleOneDatabaseContext db, IOptionsMonitor<ReportSettings> reportSettings, FileGeneratorService fileGeneratorService) =>
        {
            var receipt = db.Receipts.AsNoTracking().FirstOrDefault(r => r.id == revenueId);
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

            if (receipt.file_location == null)
            {
                //generate receipt
                var studentDetails = db.Student.AsNoTracking().FirstOrDefault(s => s.id == receipt.fk_student_id);
                var revenue = db.Revenue.AsNoTracking().FirstOrDefault(r => r.id == receipt.fk_revenue_id);

                if (studentDetails == null)
                {
                    return TypedResults.NotFound("Student details not found.");
                }

                if (revenue == null)
                {
                    return TypedResults.NotFound("Revenue record not found.");
                }

                var generateReceiptFileResult = GenerateReceiptFile(logger, studentDetails, receipt, revenue, fileGeneratorService, db);

                //update receipt record with file location
                if (generateReceiptFileResult.IsSuccess)
                {
                    receipt.file_location = generateReceiptFileResult.Value;
                    receipt.file_location_type = FileLocationType.Local;
                    db.Receipts.Update(receipt);
                    await db.SaveChangesAsync();
                }
                else
                {
                    return TypedResults.NotFound("Error generating receipt file.");
                }
            }

            if (receipt.file_location == null)
            {
                return TypedResults.NotFound("File not found.");
            }

            var filePath = receipt.file_location;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return Results.File(fileStream, "application/pdf", Path.GetFileName(receipt.file_location));
        })
        .WithName("GetReceipt")
        .WithOpenApi();

        //add expense
        group.MapPost("/Expense", async Task<Results<Created<ExpenseDto>, NotFound<string>>>
            ([FromForm] ExpenseDto expenseDto, ShuleOneDatabaseContext db, IOptionsMonitor<Settings> settings, IMapper mapper) =>
        {
            if (expenseDto.fk_from_account_id == 0)
            {
                Account defaultBankAccount = GetDefaultAccountId(PaymentMethod.Mpesa, db);
                if (defaultBankAccount == null)
                {
                    return TypedResults.NotFound("Default accounts not set.");
                }
                else
                {
                    expenseDto.fk_from_account_id = defaultBankAccount.id;
                }
            }

            var fromAccount = await db.Account
                .FirstOrDefaultAsync(a => a.id == expenseDto.fk_from_account_id);

            if (fromAccount == null)
            {
                return TypedResults.NotFound("From account not found.");
            }
            
            //get to-account with category ie the expense account
            //TODO add checks for if expense category exist and has account
            var expenseSubCategory = await db.ExpensesSubcategory.Where(es => es.id == expenseDto.fk_expense_subcategory_id).FirstAsync();
            var expenseCategoryAccount = await db.ExpensesCategory.Where(e => e.id == expenseSubCategory.fk_expense_category_id).FirstAsync();

            expenseDto.fk_to_account_id = expenseCategoryAccount.id;
            logger.LogInformation("expense to Account: " + expenseCategoryAccount.fk_account_id);
            logger.LogInformation("expense sub Account: " + expenseSubCategory.id);
            logger.LogInformation("expense fk_expense_subcategory_id: " + expenseDto.fk_expense_subcategory_id);
            
            var toAccount = await db.Account
                .FirstOrDefaultAsync(a => a.id == expenseCategoryAccount.fk_account_id);
            if (toAccount == null)
            {
                return TypedResults.NotFound("To account not found.");
            }
            
            Transaction transaction = new Transaction()
            {
                amount = expenseDto.amount,
                description = expenseDto.description,
                reference_id = expenseDto.payment_reference,
                created_by = "0",
                transaction_type = TransactionType.ExpensePaid,
                fk_transaction_type_identifier = expenseDto.fk_to_account_id,
            };

            db.Transaction.Add(transaction);
            await db.SaveChangesAsync(); // Ensure transaction.id is available

            //upload receipt 
            string recieptLocation = "";
            if(expenseDto.reciept != null)
            {
                var filePath = settings.CurrentValue.ReceiptLocation;
                
                recieptLocation = $"{filePath}/{expenseDto.description}_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}_{expenseDto.reciept.FileName.ToString()}";

                if (!Directory.Exists(recieptLocation)) { Directory.CreateDirectory(Path.GetDirectoryName(recieptLocation)); }; // Ensure folder exists

                using (var stream = new FileStream(recieptLocation, FileMode.Create))
                {
                    await expenseDto.reciept.CopyToAsync(stream);
                }
            }
            
            //record expense
            expenseDto.fk_transaction_id = transaction.id;
            Expense expense = new Expense()
            {
                amount = expenseDto.amount,
                description = expenseDto.description,
                date_created = DateTime.UtcNow,
                date_paid = expenseDto.date_paid,
                fk_from_account_id = expenseDto.fk_from_account_id,
                fk_to_account_id = expenseDto.fk_to_account_id,
                fk_transaction_id = expenseDto.fk_transaction_id,
                fk_expense_subcategory_id = expenseDto.fk_expense_subcategory_id,
                paid_by = expenseDto.paid_by,
                payment_reference = expenseDto.payment_reference,
                reciept = recieptLocation 
            };

            await db.Expenses.AddAsync(expense);
            await db.SaveChangesAsync(); // Ensure transaction.id is available


            db.JournalEntry.AddRange(new List<JournalEntry>
            {
                new JournalEntry()
                {
                    amount = expenseDto.amount,
                    fk_account_id = expenseDto.fk_from_account_id, // Main bank
                    fk_transaction_id = transaction.id,
                    fk_journal_entry_type = (int)JournalEntryType.Credit
                },
                new JournalEntry()
                {
                    amount = expenseDto.amount,
                    fk_account_id = expenseDto.fk_to_account_id, // Fees
                    fk_transaction_id = transaction.id,
                    fk_journal_entry_type = (int)JournalEntryType.Debit
                }
            });

   
            // var fromAccountEntity = await db.Account.FindAsync(expenseDto.fk_from_account_id);
            // var toAccountEntity = await db.Account.FindAsync(expenseDto.fk_to_account_id);

            var fromAccountNewBalance = fromAccount.balance - expenseDto.amount;
            var toAccountNewBalance = toAccount.balance + expenseDto.amount;
            fromAccount.balance = fromAccountNewBalance;
            toAccount.balance = toAccountNewBalance;
            db.Update(fromAccount);
            db.Update(toAccount);
            
            await db.SaveChangesAsync();

            //expenseDto = mapper.Map<ExpenseDto>(expense);
            return TypedResults.Created($"/api/Finance/{expenseDto.id}", expenseDto);
        })
        .DisableAntiforgery() // This is the equivalent of [IgnoreAntiforgeryToken] for minimal APIs
        .WithName("RecordExpense")
        .WithOpenApi();

        //get expenses
        group.MapGet("/Expense/All", async
            (ShuleOneDatabaseContext db, IMapper mapper, int pageNumber = 0, int pageSize = 1) =>
        {
            var allExpensesCount = 0;

            var query = db.Expenses
                .AsNoTracking()
                .OrderBy(c => c.id)
                .AsQueryable();

            // Apply filters dynamically based on the provided parameters
            // TODO

            // Count the total records for pagination
            allExpensesCount = await query.CountAsync();

            // Apply pagination and projection to DTO
            var allExpenses = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .OrderByDescending(e => e.date_paid)
                .ToListAsync();

            // Return results including pagination metadata
            return Results.Ok(new
            {
                total = allExpensesCount,
                expenses = allExpenses
            });
        })
        .WithName("GetExpenseRecordsPaginated")
        .WithOpenApi();

        //get revenues
        group.MapGet("/Revenue/All", async
            (ShuleOneDatabaseContext db, IMapper mapper, int pageNumber = 0, int pageSize = 1,
            string? account = null, string? transactionRef = null, DateTime? dateFrom = null, DateTime? dateTo = null) =>
        {
            var allRevenueCount = 0;

            var query = db.Revenue
                .AsNoTracking()
                .GroupJoin(
                    db.Student,
                    revenue => revenue.fk_intended_account_number,
                    student => student.id,
                    (revenue, student) => new { Revenue = revenue, Student = student.First() } // Left join effect
                )
                .OrderByDescending(r => r.Revenue.payment_date)
                .AsQueryable();


            // Apply filters dynamically based on the provided parameters
            if (account is not null)
            {
                query = query.Where(s => s.Revenue.account_number == account);
            }
            if (transactionRef is not null)
            {
                query = query.Where(s => s.Revenue.payment_reference == transactionRef);
            }
            if (dateFrom != null || dateTo != null)
            {
                query = query.Where(s => s.Revenue.payment_date >= dateFrom && s.Revenue.payment_date <= dateTo);
            }

            // Count the total records for pagination
            allRevenueCount = await query.CountAsync();

            // Apply pagination and projection to DTO
            var allRevenue = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return results including pagination metadata
            return Results.Ok(new
            {
                total = allRevenueCount,
                revenueStudentRecords = allRevenue
            });
        })
        .WithName("GetRevenueRecordsPaginated")
        .WithOpenApi();

        group.MapGet("/CheckPayment", async (ShuleOneDatabaseContext db, string transactionReference) =>
        {
            var revenue = await db.Revenue.AsNoTracking()
                .Where(r => r.payment_reference == transactionReference)
                .FirstOrDefaultAsync();

            return Results.Ok(new { status = revenue != null });
        })
        .WithName("CheckPayment")
        .WithOpenApi();

        group.MapGet("/Expense/Categories", async (ShuleOneDatabaseContext db, IMapper mapper) =>
        {
            var expenseCategories = await db.ExpensesCategory
                .Include(e => e.ExpenseSubCategories)
                .ToListAsync();

            var categories = await db.ExpensesCategory
                .Include(c => c.ExpenseSubCategories)
                .ToListAsync();

            var result = mapper.Map<List<ExpenseCategoryDto>>(categories);

            return Results.Ok(result);
        })
        .WithName("Get")
        .WithOpenApi();
      
    
    }


    #region Helper procedures

    private static Account? GetDefaultAccountId(PaymentMethod paymentMethod, ShuleOneDatabaseContext db)
    {
        Account account = db.Account.AsNoTracking().FirstOrDefault(a => a.default_source == paymentMethod);

        if (account == null)
        {
            return null;
        }

        return account;
    }

    private static async Task ProcessReceipt
        (Student studentDetails, Revenue revenue, ShuleOneDatabaseContext db,
        ILogger logger, FileGeneratorService fileGeneratorService)
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
                return File.Exists(receipt.file_location);
            default:
                return false;
        }
    }

    private static Result<string> GenerateReceiptFile(
        ILogger logger, Student studentDetails, Receipt receipt,
        Revenue revenue, FileGeneratorService fileGeneratorService, ShuleOneDatabaseContext db)
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

            if (generateFileResult.IsCompletedSuccessfully)
            {
                return Result.Success<string>(outputFilePath);
            }
            else
            {
                return Result.Failure<string>($"Error generating file. {generateFileResult.Result}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating receipt.");
            return Result.Failure<string>("Error generating file.");
        }
    }
    #endregion
}
