using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    /// <inheritdoc />
    public partial class FixedTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_receiot_item_receipt_Receiptid",
                table: "receiot_item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_receiot_item",
                table: "receiot_item");

            migrationBuilder.RenameTable(
                name: "receiot_item",
                newName: "receipt_item");

            migrationBuilder.RenameIndex(
                name: "IX_receiot_item_Receiptid",
                table: "receipt_item",
                newName: "IX_receipt_item_Receiptid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_receipt_item",
                table: "receipt_item",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_receipt_item_receipt_Receiptid",
                table: "receipt_item",
                column: "Receiptid",
                principalTable: "receipt",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_receipt_item_receipt_Receiptid",
                table: "receipt_item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_receipt_item",
                table: "receipt_item");

            migrationBuilder.RenameTable(
                name: "receipt_item",
                newName: "receiot_item");

            migrationBuilder.RenameIndex(
                name: "IX_receipt_item_Receiptid",
                table: "receiot_item",
                newName: "IX_receiot_item_Receiptid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_receiot_item",
                table: "receiot_item",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_receiot_item_receipt_Receiptid",
                table: "receiot_item",
                column: "Receiptid",
                principalTable: "receipt",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
