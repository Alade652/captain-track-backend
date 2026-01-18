using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class addisavailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "WaterSuppliers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "TowTruckOperators",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "RiderorParks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "HouseMover",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "HouseCleaners",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "GasSuppliers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "DryCleaners",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "CarWashers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Ambulances",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "WaterSuppliers");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "TowTruckOperators");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "RiderorParks");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "HouseMover");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "HouseCleaners");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "GasSuppliers");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "DryCleaners");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "CarWashers");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Ambulances");

        }
    }
}
