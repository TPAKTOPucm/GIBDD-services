using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Vehicles_VehicleId",
                table: "Fines");

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DriverId",
                table: "VehicleRegistrations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "VehicleRegistrations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "VehicleRegistrations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "AccountCode",
                table: "PaymentReceipts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BankCode",
                table: "PaymentReceipts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "PaymentReceipts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "Fines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Fines",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "Fines",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Fines",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_CityType",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Address_Home",
                table: "Drivers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Address_Office",
                table: "Drivers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Address_ZipCode",
                table: "Drivers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Drivers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FullName_FirstName",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName_LastName",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName_MiddleName",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRegistrations_DriverId",
                table: "VehicleRegistrations",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRegistrations_VehicleId",
                table: "VehicleRegistrations",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Vehicles_VehicleId",
                table: "Fines",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleRegistrations_Drivers_DriverId",
                table: "VehicleRegistrations",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleRegistrations_Vehicles_VehicleId",
                table: "VehicleRegistrations",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Vehicles_VehicleId",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleRegistrations_Drivers_DriverId",
                table: "VehicleRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleRegistrations_Vehicles_VehicleId",
                table: "VehicleRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VehicleRegistrations_DriverId",
                table: "VehicleRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VehicleRegistrations_VehicleId",
                table: "VehicleRegistrations");

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "VehicleRegistrations");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "VehicleRegistrations");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "VehicleRegistrations");

            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "PaymentReceipts");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "PaymentReceipts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PaymentReceipts");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Address_CityType",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Address_Home",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Address_Office",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Address_ZipCode",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "FullName_FirstName",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "FullName_LastName",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "FullName_MiddleName",
                table: "Drivers");

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "Fines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Vehicles_VehicleId",
                table: "Fines",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }
    }
}
