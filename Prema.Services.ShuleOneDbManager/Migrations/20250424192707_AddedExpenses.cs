using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class AddedExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expense_types");

            migrationBuilder.CreateTable(
                name: "expense",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    payment_reference = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fk_from_account_id = table.Column<int>(type: "int", nullable: false),
                    fk_to_account_id = table.Column<int>(type: "int", nullable: false),
                    fk_transaction_id = table.Column<int>(type: "int", nullable: true),
                    paid_by = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_paid = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reciept = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense", x => x.id);
                    table.ForeignKey(
                        name: "FK_expense_account_fk_from_account_id",
                        column: x => x.fk_from_account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_expense_account_fk_to_account_id",
                        column: x => x.fk_to_account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_expense_transaction_fk_transaction_id",
                        column: x => x.fk_transaction_id,
                        principalTable: "transaction",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_expense_fk_from_account_id",
                table: "expense",
                column: "fk_from_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_fk_to_account_id",
                table: "expense",
                column: "fk_to_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_fk_transaction_id",
                table: "expense",
                column: "fk_transaction_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expense");

            migrationBuilder.CreateTable(
                name: "expense_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    fk_from_account_id = table.Column<int>(type: "int", nullable: false),
                    fk_to_account_id = table.Column<int>(type: "int", nullable: false),
                    added_by = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_types", x => x.id);
                    table.ForeignKey(
                        name: "FK_expense_types_account_fk_from_account_id",
                        column: x => x.fk_from_account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_expense_types_account_fk_to_account_id",
                        column: x => x.fk_to_account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_expense_types_fk_from_account_id",
                table: "expense_types",
                column: "fk_from_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_types_fk_to_account_id",
                table: "expense_types",
                column: "fk_to_account_id");
        }
    }
}
