using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class addservices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceProviding",
                table: "HouseMover");

            migrationBuilder.DropColumn(
                name: "ServiceProviding",
                table: "HouseCleaners");

            migrationBuilder.DropColumn(
                name: "ServiceProviding",
                table: "GasSuppliers");

            migrationBuilder.DropColumn(
                name: "ServiceProviding",
                table: "DryCleaners");

            migrationBuilder.DropColumn(
                name: "ServiceProviding",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "ServiceProviding",
                table: "CarWashers");

            migrationBuilder.DropColumn(
                name: "ServiceProviding",
                table: "Ambulances");

            migrationBuilder.CreateTable(
                name: "ServiceProvidings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServiceProvidingId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleteOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleteBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProvidings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProvidings_ServiceProvidings_ServiceProvidingId",
                        column: x => x.ServiceProvidingId,
                        principalTable: "ServiceProvidings",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserServiceProvidings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServiceProvidingId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleteOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleteBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServiceProvidings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserServiceProvidings_ServiceProvidings_ServiceProvidingId",
                        column: x => x.ServiceProvidingId,
                        principalTable: "ServiceProvidings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServiceProvidings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProvidings_ServiceProvidingId",
                table: "ServiceProvidings",
                column: "ServiceProvidingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServiceProvidings_ServiceProvidingId",
                table: "UserServiceProvidings",
                column: "ServiceProvidingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServiceProvidings_UserId",
                table: "UserServiceProvidings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserServiceProvidings");

            migrationBuilder.DropTable(
                name: "ServiceProvidings");

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviding",
                table: "HouseMover",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviding",
                table: "HouseCleaners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviding",
                table: "GasSuppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviding",
                table: "DryCleaners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviding",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviding",
                table: "CarWashers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviding",
                table: "Ambulances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
