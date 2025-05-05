using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class AddedExpenseCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "category",
                table: "expense");

            migrationBuilder.AddColumn<int>(
                name: "fk_expense_subcategory_id",
                table: "expense",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "expense_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_account_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_category", x => x.id);
                    table.ForeignKey(
                        name: "FK_expense_category_account_fk_account_id",
                        column: x => x.fk_account_id,
                        principalTable: "account",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "expense_subcategory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_expense_category_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_subcategory", x => x.id);
                    table.ForeignKey(
                        name: "FK_expense_subcategory_expense_category_fk_expense_category_id",
                        column: x => x.fk_expense_category_id,
                        principalTable: "expense_category",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_expense_fk_expense_subcategory_id",
                table: "expense",
                column: "fk_expense_subcategory_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_category_fk_account_id",
                table: "expense_category",
                column: "fk_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_subcategory_fk_expense_category_id",
                table: "expense_subcategory",
                column: "fk_expense_category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_expense_expense_subcategory_fk_expense_subcategory_id",
                table: "expense",
                column: "fk_expense_subcategory_id",
                principalTable: "expense_subcategory",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expense_expense_subcategory_fk_expense_subcategory_id",
                table: "expense");

            migrationBuilder.DropTable(
                name: "expense_subcategory");

            migrationBuilder.DropTable(
                name: "expense_category");

            migrationBuilder.DropIndex(
                name: "IX_expense_fk_expense_subcategory_id",
                table: "expense");

            migrationBuilder.DropColumn(
                name: "fk_expense_subcategory_id",
                table: "expense");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "expense",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
