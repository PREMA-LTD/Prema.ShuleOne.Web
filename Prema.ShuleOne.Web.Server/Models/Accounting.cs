using Prema.ShuleOne.Web.Server.Models.BaseTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prema.ShuleOne.Web.Server.Models
{
    [Table("account")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [MaxLength(255)]
        public string name { get; set; }
        [Required]
        public AccountType type { get; set; }
        public PaymentMethod? default_source { get; set; } //unique and nullable
        [Column(TypeName = "decimal(18,2)")]
        public decimal balance { get; set; } = 0.00m;
        public string created_by { get; set; } //if system generated, this will be 0 else user id
        public DateTime date_created { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<JournalEntry> JournalEntries { get; set; }
        //public ICollection<GeneralLedger> GeneralLedger { get; set; }
    }


    [Table("account_type")]
    public class AccountTypes : BaseTypeNoTracking
    {
    }
    public enum AccountType
    {
        Asset =  0,
        Liability = 1,
        Equity = 2,
        Revenue = 3,
        Expense = 4
    }

    [Table("transaction")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string reference_id { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }
        //[Required]
        //public TransactionType transaction_type { get; set; }
        //[Required]
        //public int fk_account_id { get; set; }
        public string created_by { get; set; } //if system generated, this will be 0 else user id
        public DateTime date_created { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        //[ForeignKey("fk_account_id")]
        //public Account Account { get; set; }
    }

    [Table("journal_entry_type")]
    public class JournalEntryTypes : BaseTypeNoTracking
    {
    }
    public enum JournalEntryType
    {
        Debit = 0,
        Credit = 1
    }

    [Table("general_ledger")]
    public class JournalEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int fk_transaction_id { get; set; }
        [Required]
        public int fk_account_id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; } = 0.00m;
        [Required]
        public JournalEntryType type { get; set; }
        public DateTime date_created { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("fk_transaction_id")]
        public Transaction Transaction { get; set; }
        [ForeignKey("fk_account_id")]
        public Account Account { get; set; }
    }


    //[Table("general_ledger")]
    //public class GeneralLedger
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }
    //    [Required]
    //    public int fk_account_id { get; set; }
    //    [Required]
    //    public DateTime date_created { get; set; }
    //    [Required]
    //    public string description { get; set; }
    //    [Column(TypeName = "decimal(18,2)")]
    //    public decimal debit { get; set; } = 0.00m;
    //    [Column(TypeName = "decimal(18,2)")]
    //    public decimal credit { get; set; } = 0.00m;
    //    [Column(TypeName = "decimal(18,2)")]
    //    public decimal balance { get; set; }

    //    // Navigation Property
    //    [ForeignKey("fk_account_id")]
    //    public Account Account { get; set; }
    //}

    #region Invoice not needed now
    //[Table("invoice")]
    //public class Invoice
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }
    //    [Required]
    //    [MaxLength(50)]
    //    public string invoice_number { get; set; }
    //    [Required]
    //    [MaxLength(255)]
    //    public string client_name { get; set; }
    //    [Required]
    //    public decimal total_amount { get; set; }
    //    [Required]
    //    public DateTime due_date { get; set; }
    //    public InvoiceStatus status { get; set; } = InvoiceStatus.Pending;
    //    public DateTime date_created { get; set; } = DateTime.UtcNow;

    //    // Navigation Properties
    //    public ICollection<Revenue> Revenue { get; set; }
    //}

    //[Table("invoice_status")]
    //public class InvoiceStatuses : BaseTypeNoTracking
    //{
    //}

    //public enum InvoiceStatus
    //{
    //    Pending = 0,
    //    Paid = 1,
    //    Overdue = 2
    //}
    #endregion

    [Table("revenue")]
    public class Revenue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        //[Required]
        //public int fk_invoice_id { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }
        public string paid_by { get; set; }
        public string payment_reference { get; set; }
        public string account_number { get; set; }
        public RevenueStatus status { get; set; } = RevenueStatus.Unallocated;
        [Required]
        public DateTime payment_date { get; set; }
        [Required]
        public PaymentMethod payment_method { get; set; }
        public DateTime date_created { get; set; } = DateTime.UtcNow;
        public string recorded_by { get; set; } //if system generated, this will be 0 else user id

        // Navigation Property
        //public Invoice Invoice { get; set; }
    }
    [Table("revenue_status")]
    public class RevenueStatuses : BaseTypeNoTracking
    {
    }
    public enum RevenueStatus
    {
        Unallocated = 0,
        Allocated = 1,
        TransactionPending = 2
    }


    [Table("payment_methods")]
    public class PaymentMethods : BaseTypeNoTracking
    {
    }
    public enum PaymentMethod
    {
        Mpesa = 0,
        CardPDQ = 1,
        BankTransfer = 2
    }

    [Table("expense")]
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }
        [Required]
        [MaxLength(100)]
        public string category { get; set; }
        [Required]
        public string paid_by { get; set; }
        [Required]
        public DateTime date_paid { get; set; }
        public DateTime date_created { get; set; } = DateTime.UtcNow;
    }



    [Table("receipt")]
    public class Receipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int fk_student_id { get; set; }
        public int fk_revenue_id { get; set; }
        public string? file_location { get; set; }
        public FileLocationType? file_location_type { get; set; }
        public ReceiptStatus status { get; set; };
        public DateTime date_created { get; set; } = DateTime.UtcNow;

        public ICollection<ReceiptItem> ReceiptItems { get; set; }

        [ForeignKey("fk_revenue_id")]
        public Revenue Revenue { get; set; }
        [ForeignKey("fk_student_id")]
        public Student Student { get; set; }    
    }

    [Table("receipt_item")]
    public class ReceiptItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }
        public ReceiptItemType item_type { get; set; }
        public int fk_receipt_id { get; set; }
        public DateTime date_created { get; set; } = DateTime.UtcNow;

        [ForeignKey("fk_receipt_id")]
        public Receipt Receipt { get; set; }
    }

    [Table("receipt_status")]
    public class ReceiptStatuses : BaseTypeNoTracking
    {
    }
    public enum ReceiptStatus
    {
        InValid = 0,
        Valid = 1
    }

    [Table("receipt_item_type")]
    public class ReceiptItemTypes : BaseTypeNoTracking
    {
    }
    public enum ReceiptItemType
    {
        Generic = 0,
        Tution = 1,
        Transport = 2,
        Boarding = 3,
        Tour = 4,
        Computer = 5,
        Sports = 6
    }

    [Table("file_location_types")]
    public class FileLocationTypes : BaseTypeNoTracking
    {
    }
    public enum FileLocationType
    {
        Local = 0,
        ZohoDrive = 1,
        GoogleDrive = 2,
        S3Bucket = 3
    }


}
