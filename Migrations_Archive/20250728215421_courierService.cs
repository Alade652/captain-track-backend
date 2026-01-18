using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class courierService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courier_Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CustomerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RiderorParkId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PickupLocation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Destination = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServiceStatus = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Duration = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DistanceToPickupLocation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DurationToPickupLocation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstimatedPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NegotiatedPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DeliveryType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Courier_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courier_Services_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courier_Services_RiderorParks_RiderorParkId",
                        column: x => x.RiderorParkId,
                        principalTable: "RiderorParks",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourierNegotiations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Courier_ServiceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RiderorParkId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CustomerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NegotiatingPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Acceptedprice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
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
                    table.PrimaryKey("PK_CourierNegotiations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourierNegotiations_Courier_Services_Courier_ServiceId",
                        column: x => x.Courier_ServiceId,
                        principalTable: "Courier_Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourierNegotiations_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourierNegotiations_RiderorParks_RiderorParkId",
                        column: x => x.RiderorParkId,
                        principalTable: "RiderorParks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Courier_Services_CustomerId",
                table: "Courier_Services",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Courier_Services_RiderorParkId",
                table: "Courier_Services",
                column: "RiderorParkId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierNegotiations_Courier_ServiceId",
                table: "CourierNegotiations",
                column: "Courier_ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierNegotiations_CustomerId",
                table: "CourierNegotiations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierNegotiations_RiderorParkId",
                table: "CourierNegotiations",
                column: "RiderorParkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourierNegotiations");

            migrationBuilder.DropTable(
                name: "Courier_Services");
        }
    }
}
