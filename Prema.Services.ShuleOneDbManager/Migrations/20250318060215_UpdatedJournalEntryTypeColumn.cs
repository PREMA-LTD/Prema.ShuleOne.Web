using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedJournalEntryTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "general_ledger",
                newName: "fk_journal_entry_type");

            migrationBuilder.CreateIndex(
                name: "IX_general_ledger_fk_journal_entry_type",
                table: "general_ledger",
                column: "fk_journal_entry_type");

            migrationBuilder.AddForeignKey(
                name: "FK_general_ledger_journal_entry_type_fk_journal_entry_type",
                table: "general_ledger",
                column: "fk_journal_entry_type",
                principalTable: "journal_entry_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_general_ledger_journal_entry_type_fk_journal_entry_type",
                table: "general_ledger");

            migrationBuilder.DropIndex(
                name: "IX_general_ledger_fk_journal_entry_type",
                table: "general_ledger");

            migrationBuilder.RenameColumn(
                name: "fk_journal_entry_type",
                table: "general_ledger",
                newName: "type");
        }
    }
}
