using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;
using Prema.ShuleOne.Web.Server.Database.LocationData;
using Prema.ShuleOne.Web.Server.Models.Location;
using Prema.ShuleOne.Web.Server.Models;


namespace Prema.ShuleOne.Web.Server.Database
{
    public partial class ShuleOneDatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ShuleOneDatabaseContext(DbContextOptions<ShuleOneDatabaseContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>()
                .HasIndex(u => u.default_source)
                .IsUnique();

            base.OnModelCreating(builder);

            this.OnModelBuilding(builder);
        }

        public DbSet<County> County { get; set; }
        public DbSet<Subcounty> Subcounty { get; set; }
        public DbSet<Ward> Ward { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeBankDetails> EmployeeBankDetails { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentContact> StudentContact { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<SMSRecord> SMSRecord { get; set; }
        public DbSet<SMSFailure> SMSFailure { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public DbSet<TutionFeesStructure> TutionFeesStructure { get; set; }
        public DbSet<FeeType> FeeType { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<JournalEntry> JournalEntry { get; set; }
        //public DbSet<GeneralLedger> GeneralLedger { get; set; }
        //public DbSet<Invoice> Invoice { get; set; }
        public DbSet<Revenue> Revenue { get; set; }
        //public DbSet<Expense> Expense { get; set; }
        public DbSet<AccountTypes> AccountTypes { get; set; }
        public DbSet<JournalEntryTypes> JournalEntryTypes { get; set; }
        //public DbSet<InvoiceStatuses> InvoiceStatuses { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<ReceiptItemTypes> ReceiptItemTypes { get; set; }
        public DbSet<FileLocationTypes> FileLocationTypes { get; set; }
        public DbSet<RevenueStatuses> RevenueStatuses { get; set; }
        public DbSet<ReceiptStatuses> ReceiptStatuses { get; set; }
        public DbSet<TransactionTypes> TransactionTypes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<AccountCategories> AccountCategories { get; set; }
    }
}