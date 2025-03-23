using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedExpnses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "fk_from_account_id",
                table: "expense",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "fk_to_account_id",
                table: "expense",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "payment_reference",
                table: "expense",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_expense_fk_from_account_id",
                table: "expense",
                column: "fk_from_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_fk_to_account_id",
                table: "expense",
                column: "fk_to_account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_expense_account_fk_from_account_id",
                table: "expense",
                column: "fk_from_account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_expense_account_fk_to_account_id",
                table: "expense",
                column: "fk_to_account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expense_account_fk_from_account_id",
                table: "expense");

            migrationBuilder.DropForeignKey(
                name: "FK_expense_account_fk_to_account_id",
                table: "expense");

            migrationBuilder.DropIndex(
                name: "IX_expense_fk_from_account_id",
                table: "expense");

            migrationBuilder.DropIndex(
                name: "IX_expense_fk_to_account_id",
                table: "expense");

            migrationBuilder.DropColumn(
                name: "fk_from_account_id",
                table: "expense");

            migrationBuilder.DropColumn(
                name: "fk_to_account_id",
                table: "expense");

            migrationBuilder.DropColumn(
                name: "payment_reference",
                table: "expense");
        }
    }
}
