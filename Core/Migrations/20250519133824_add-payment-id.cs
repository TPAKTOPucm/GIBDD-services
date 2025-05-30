using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addpaymentid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptId",
                table: "Fines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Fines_ReceiptId",
                table: "Fines",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_PaymentReceipts_ReceiptId",
                table: "Fines",
                column: "ReceiptId",
                principalTable: "PaymentReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_PaymentReceipts_ReceiptId",
                table: "Fines");

            migrationBuilder.DropIndex(
                name: "IX_Fines_ReceiptId",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "Fines");
        }
    }
}
