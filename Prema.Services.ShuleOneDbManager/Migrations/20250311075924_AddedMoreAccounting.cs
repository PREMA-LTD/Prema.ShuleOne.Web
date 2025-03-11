using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoreAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_revenue_invoice_Invoiceid",
                table: "revenue");

            migrationBuilder.DropTable(
                name: "invoice");

            migrationBuilder.DropTable(
                name: "invoice_status");

            migrationBuilder.DropIndex(
                name: "IX_revenue_Invoiceid",
                table: "revenue");

            migrationBuilder.DropColumn(
                name: "Invoiceid",
                table: "revenue");

            migrationBuilder.DropColumn(
                name: "fk_invoice_id",
                table: "revenue");

            migrationBuilder.RenameColumn(
                name: "fk_created_by",
                table: "transaction",
                newName: "created_by");

            migrationBuilder.AddColumn<string>(
                name: "account_number",
                table: "revenue",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "paid_by",
                table: "revenue",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "payment_reference",
                table: "revenue",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "recorded_by",
                table: "revenue",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "account",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receipt",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_student_id = table.Column<int>(type: "int", nullable: false),
                    fk_revenue_id = table.Column<int>(type: "int", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Revenueid = table.Column<int>(type: "int", nullable: false),
                    Studentid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt", x => x.id);
                    table.ForeignKey(
                        name: "FK_receipt_revenue_Revenueid",
                        column: x => x.Revenueid,
                        principalTable: "revenue",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_receipt_student_Studentid",
                        column: x => x.Studentid,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receipt_item_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt_item_type", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receiot_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    item_type = table.Column<int>(type: "int", nullable: false),
                    fk_receipt_id = table.Column<int>(type: "int", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Receiptid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receiot_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_receiot_item_receipt_Receiptid",
                        column: x => x.Receiptid,
                        principalTable: "receipt",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_receiot_item_Receiptid",
                table: "receiot_item",
                column: "Receiptid");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_Revenueid",
                table: "receipt",
                column: "Revenueid");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_Studentid",
                table: "receipt",
                column: "Studentid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "receiot_item");

            migrationBuilder.DropTable(
                name: "receipt_item_type");

            migrationBuilder.DropTable(
                name: "receipt");

            migrationBuilder.DropColumn(
                name: "account_number",
                table: "revenue");

            migrationBuilder.DropColumn(
                name: "paid_by",
                table: "revenue");

            migrationBuilder.DropColumn(
                name: "payment_reference",
                table: "revenue");

            migrationBuilder.DropColumn(
                name: "recorded_by",
                table: "revenue");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "account");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "transaction",
                newName: "fk_created_by");

            migrationBuilder.AddColumn<int>(
                name: "Invoiceid",
                table: "revenue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "fk_invoice_id",
                table: "revenue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    client_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    due_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    invoice_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "invoice_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_status", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_revenue_Invoiceid",
                table: "revenue",
                column: "Invoiceid");

            migrationBuilder.AddForeignKey(
                name: "FK_revenue_invoice_Invoiceid",
                table: "revenue",
                column: "Invoiceid",
                principalTable: "invoice",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
