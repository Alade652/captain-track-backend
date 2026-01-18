using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class twuhhwdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "Courier_Services",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Courier_Services_VehicleId",
                table: "Courier_Services",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courier_Services_CourierVehicles_VehicleId",
                table: "Courier_Services",
                column: "VehicleId",
                principalTable: "CourierVehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courier_Services_CourierVehicles_VehicleId",
                table: "Courier_Services");

            migrationBuilder.DropIndex(
                name: "IX_Courier_Services_VehicleId",
                table: "Courier_Services");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Courier_Services");
        }
    }
}
