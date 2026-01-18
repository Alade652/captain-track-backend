using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class carwashnego : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NegotiatedPrice",
                table: "CarWashNegotiations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeRequired",
                table: "CarWashNegotiations",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedPrice",
                table: "CarWashings",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NegotiatedPrice",
                table: "CarWashings",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeRequired",
                table: "CarWashNegotiations");

            migrationBuilder.DropColumn(
                name: "EstimatedPrice",
                table: "CarWashings");

            migrationBuilder.DropColumn(
                name: "NegotiatedPrice",
                table: "CarWashings");

            migrationBuilder.AlterColumn<decimal>(
                name: "NegotiatedPrice",
                table: "CarWashNegotiations",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }
    }
}
