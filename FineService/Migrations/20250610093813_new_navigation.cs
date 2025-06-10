using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FineService.Migrations
{
    /// <inheritdoc />
    public partial class new_navigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_PaymentReceipts_PaymentReceiptId",
                table: "Fines");

            migrationBuilder.DropIndex(
                name: "IX_Fines_PaymentReceiptId",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "PaymentReceiptId",
                table: "Fines");

            migrationBuilder.AddColumn<Guid>(
                name: "FineId",
                table: "PaymentReceipts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_FineId",
                table: "PaymentReceipts",
                column: "FineId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_Fines_FineId",
                table: "PaymentReceipts",
                column: "FineId",
                principalTable: "Fines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_Fines_FineId",
                table: "PaymentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_PaymentReceipts_FineId",
                table: "PaymentReceipts");

            migrationBuilder.DropColumn(
                name: "FineId",
                table: "PaymentReceipts");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentReceiptId",
                table: "Fines",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fines_PaymentReceiptId",
                table: "Fines",
                column: "PaymentReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_PaymentReceipts_PaymentReceiptId",
                table: "Fines",
                column: "PaymentReceiptId",
                principalTable: "PaymentReceipts",
                principalColumn: "Id");
        }
    }
}
