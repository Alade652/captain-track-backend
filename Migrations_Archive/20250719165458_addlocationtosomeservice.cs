using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class addlocationtosomeservice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistancetoLocation",
                table: "HouseCleanings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DuraiontoLocation",
                table: "HouseCleanings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "HouseCleanings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DistancetoLocation",
                table: "GasDeliveries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DuraiontoLocation",
                table: "GasDeliveries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DistancetoLocation",
                table: "DryCleanings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DuraiontoLocation",
                table: "DryCleanings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DistancetoLocation",
                table: "CarWashings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DuraiontoLocation",
                table: "CarWashings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "CarWashings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistancetoLocation",
                table: "HouseCleanings");

            migrationBuilder.DropColumn(
                name: "DuraiontoLocation",
                table: "HouseCleanings");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "HouseCleanings");

            migrationBuilder.DropColumn(
                name: "DistancetoLocation",
                table: "GasDeliveries");

            migrationBuilder.DropColumn(
                name: "DuraiontoLocation",
                table: "GasDeliveries");

            migrationBuilder.DropColumn(
                name: "DistancetoLocation",
                table: "DryCleanings");

            migrationBuilder.DropColumn(
                name: "DuraiontoLocation",
                table: "DryCleanings");

            migrationBuilder.DropColumn(
                name: "DistancetoLocation",
                table: "CarWashings");

            migrationBuilder.DropColumn(
                name: "DuraiontoLocation",
                table: "CarWashings");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "CarWashings");
        }
    }
}
