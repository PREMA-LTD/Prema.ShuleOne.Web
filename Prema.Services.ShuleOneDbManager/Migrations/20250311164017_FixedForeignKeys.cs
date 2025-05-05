using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class FixedForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_general_ledger_account_Accountid",
                table: "general_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_journal_entry_account_Accountid",
                table: "journal_entry");

            migrationBuilder.DropForeignKey(
                name: "FK_journal_entry_transaction_Transactionid",
                table: "journal_entry");

            migrationBuilder.DropForeignKey(
                name: "FK_receipt_revenue_Revenueid",
                table: "receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_receipt_student_Studentid",
                table: "receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_receipt_item_receipt_Receiptid",
                table: "receipt_item");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_account_Accountid",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_transaction_Accountid",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_receipt_item_Receiptid",
                table: "receipt_item");

            migrationBuilder.DropIndex(
                name: "IX_receipt_Revenueid",
                table: "receipt");

            migrationBuilder.DropIndex(
                name: "IX_receipt_Studentid",
                table: "receipt");

            migrationBuilder.DropIndex(
                name: "IX_journal_entry_Accountid",
                table: "journal_entry");

            migrationBuilder.DropIndex(
                name: "IX_journal_entry_Transactionid",
                table: "journal_entry");

            migrationBuilder.DropIndex(
                name: "IX_general_ledger_Accountid",
                table: "general_ledger");

            migrationBuilder.DropColumn(
                name: "Accountid",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "Receiptid",
                table: "receipt_item");

            migrationBuilder.DropColumn(
                name: "Revenueid",
                table: "receipt");

            migrationBuilder.DropColumn(
                name: "Studentid",
                table: "receipt");

            migrationBuilder.DropColumn(
                name: "Accountid",
                table: "journal_entry");

            migrationBuilder.DropColumn(
                name: "Transactionid",
                table: "journal_entry");

            migrationBuilder.DropColumn(
                name: "Accountid",
                table: "general_ledger");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_fk_account_id",
                table: "transaction",
                column: "fk_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_item_fk_receipt_id",
                table: "receipt_item",
                column: "fk_receipt_id");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_fk_revenue_id",
                table: "receipt",
                column: "fk_revenue_id");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_fk_student_id",
                table: "receipt",
                column: "fk_student_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_fk_account_id",
                table: "journal_entry",
                column: "fk_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_fk_transaction_id",
                table: "journal_entry",
                column: "fk_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_general_ledger_fk_account_id",
                table: "general_ledger",
                column: "fk_account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_general_ledger_account_fk_account_id",
                table: "general_ledger",
                column: "fk_account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_journal_entry_account_fk_account_id",
                table: "journal_entry",
                column: "fk_account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_journal_entry_transaction_fk_transaction_id",
                table: "journal_entry",
                column: "fk_transaction_id",
                principalTable: "transaction",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_receipt_revenue_fk_revenue_id",
                table: "receipt",
                column: "fk_revenue_id",
                principalTable: "revenue",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_receipt_student_fk_student_id",
                table: "receipt",
                column: "fk_student_id",
                principalTable: "student",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_receipt_item_receipt_fk_receipt_id",
                table: "receipt_item",
                column: "fk_receipt_id",
                principalTable: "receipt",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_account_fk_account_id",
                table: "transaction",
                column: "fk_account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_general_ledger_account_fk_account_id",
                table: "general_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_journal_entry_account_fk_account_id",
                table: "journal_entry");

            migrationBuilder.DropForeignKey(
                name: "FK_journal_entry_transaction_fk_transaction_id",
                table: "journal_entry");

            migrationBuilder.DropForeignKey(
                name: "FK_receipt_revenue_fk_revenue_id",
                table: "receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_receipt_student_fk_student_id",
                table: "receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_receipt_item_receipt_fk_receipt_id",
                table: "receipt_item");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_account_fk_account_id",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_transaction_fk_account_id",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_receipt_item_fk_receipt_id",
                table: "receipt_item");

            migrationBuilder.DropIndex(
                name: "IX_receipt_fk_revenue_id",
                table: "receipt");

            migrationBuilder.DropIndex(
                name: "IX_receipt_fk_student_id",
                table: "receipt");

            migrationBuilder.DropIndex(
                name: "IX_journal_entry_fk_account_id",
                table: "journal_entry");

            migrationBuilder.DropIndex(
                name: "IX_journal_entry_fk_transaction_id",
                table: "journal_entry");

            migrationBuilder.DropIndex(
                name: "IX_general_ledger_fk_account_id",
                table: "general_ledger");

            migrationBuilder.AddColumn<int>(
                name: "Accountid",
                table: "transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Receiptid",
                table: "receipt_item",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Revenueid",
                table: "receipt",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Studentid",
                table: "receipt",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Accountid",
                table: "journal_entry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Transactionid",
                table: "journal_entry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Accountid",
                table: "general_ledger",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_Accountid",
                table: "transaction",
                column: "Accountid");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_item_Receiptid",
                table: "receipt_item",
                column: "Receiptid");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_Revenueid",
                table: "receipt",
                column: "Revenueid");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_Studentid",
                table: "receipt",
                column: "Studentid");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_Accountid",
                table: "journal_entry",
                column: "Accountid");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_Transactionid",
                table: "journal_entry",
                column: "Transactionid");

            migrationBuilder.CreateIndex(
                name: "IX_general_ledger_Accountid",
                table: "general_ledger",
                column: "Accountid");

            migrationBuilder.AddForeignKey(
                name: "FK_general_ledger_account_Accountid",
                table: "general_ledger",
                column: "Accountid",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_journal_entry_account_Accountid",
                table: "journal_entry",
                column: "Accountid",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_journal_entry_transaction_Transactionid",
                table: "journal_entry",
                column: "Transactionid",
                principalTable: "transaction",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_receipt_revenue_Revenueid",
                table: "receipt",
                column: "Revenueid",
                principalTable: "revenue",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_receipt_student_Studentid",
                table: "receipt",
                column: "Studentid",
                principalTable: "student",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_receipt_item_receipt_Receiptid",
                table: "receipt_item",
                column: "Receiptid",
                principalTable: "receipt",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_account_Accountid",
                table: "transaction",
                column: "Accountid",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
