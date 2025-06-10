using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfiscationService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiscationOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LicensePlate_BaseNumber = table.Column<string>(type: "text", nullable: false),
                    LicensePlate_Region = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    ImpoundYardAddress_ZipCode = table.Column<long>(type: "bigint", nullable: true),
                    ImpoundYardAddress_City = table.Column<string>(type: "text", nullable: true),
                    ImpoundYardAddress_CityType = table.Column<string>(type: "text", nullable: true),
                    ImpoundYardAddress_Street = table.Column<string>(type: "text", nullable: true),
                    ImpoundYardAddress_Home = table.Column<long>(type: "bigint", nullable: true),
                    IsReturned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiscationOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LicensePlate_BaseNumber = table.Column<string>(type: "text", nullable: false),
                    LicensePlate_Region = table.Column<long>(type: "bigint", nullable: false),
                    UnpaidFineCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiscationOrders");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
