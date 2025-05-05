using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transaction_account_fk_account_id",
                table: "transaction");

            //migrationBuilder.DropTable(
            //    name: "journal_entry");

            //migrationBuilder.DropTable(
            //    name: "transaction_type");

            migrationBuilder.DropIndex(
                name: "IX_transaction_fk_account_id",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "fk_account_id",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "transaction_type",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "balance",
                table: "general_ledger");

            migrationBuilder.DropColumn(
                name: "credit",
                table: "general_ledger");

            migrationBuilder.DropColumn(
                name: "description",
                table: "general_ledger");

            migrationBuilder.RenameColumn(
                name: "debit",
                table: "general_ledger",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "account_type",
                table: "account",
                newName: "type");

            migrationBuilder.AddColumn<int>(
                name: "Accountid",
                table: "transaction",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "revenue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "receipt",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "fk_transaction_id",
                table: "general_ledger",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "general_ledger",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "default_source",
                table: "account",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "journal_entry_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_entry_type", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receipt_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt_status", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "revenue_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revenue_status", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_Accountid",
                table: "transaction",
                column: "Accountid");

            migrationBuilder.CreateIndex(
                name: "IX_general_ledger_fk_transaction_id",
                table: "general_ledger",
                column: "fk_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_account_default_source",
                table: "account",
                column: "default_source",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_general_ledger_transaction_fk_transaction_id",
                table: "general_ledger",
                column: "fk_transaction_id",
                principalTable: "transaction",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_account_Accountid",
                table: "transaction",
                column: "Accountid",
                principalTable: "account",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_general_ledger_transaction_fk_transaction_id",
                table: "general_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_account_Accountid",
                table: "transaction");

            migrationBuilder.DropTable(
                name: "journal_entry_type");

            migrationBuilder.DropTable(
                name: "receipt_status");

            migrationBuilder.DropTable(
                name: "revenue_status");

            migrationBuilder.DropIndex(
                name: "IX_transaction_Accountid",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_general_ledger_fk_transaction_id",
                table: "general_ledger");

            migrationBuilder.DropIndex(
                name: "IX_account_default_source",
                table: "account");

            migrationBuilder.DropColumn(
                name: "Accountid",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "status",
                table: "revenue");

            migrationBuilder.DropColumn(
                name: "status",
                table: "receipt");

            migrationBuilder.DropColumn(
                name: "fk_transaction_id",
                table: "general_ledger");

            migrationBuilder.DropColumn(
                name: "type",
                table: "general_ledger");

            migrationBuilder.DropColumn(
                name: "default_source",
                table: "account");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "general_ledger",
                newName: "debit");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "account",
                newName: "account_type");

            migrationBuilder.AddColumn<int>(
                name: "fk_account_id",
                table: "transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "transaction_type",
                table: "transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "balance",
                table: "general_ledger",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "credit",
                table: "general_ledger",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "general_ledger",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "journal_entry",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_account_id = table.Column<int>(type: "int", nullable: false),
                    fk_transaction_id = table.Column<int>(type: "int", nullable: false),
                    credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_entry", x => x.id);
                    table.ForeignKey(
                        name: "FK_journal_entry_account_fk_account_id",
                        column: x => x.fk_account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_journal_entry_transaction_fk_transaction_id",
                        column: x => x.fk_transaction_id,
                        principalTable: "transaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transaction_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_type", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_fk_account_id",
                table: "transaction",
                column: "fk_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_fk_account_id",
                table: "journal_entry",
                column: "fk_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_fk_transaction_id",
                table: "journal_entry",
                column: "fk_transaction_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_account_fk_account_id",
                table: "transaction",
                column: "fk_account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
