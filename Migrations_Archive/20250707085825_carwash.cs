using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class carwash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CarWashers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CompanyName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RCorNIN = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddressorLocation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BusinessContact = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Owner = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BusinessLogoorImageUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Locations = table.Column<int>(type: "int", nullable: false),
                    YearsOfService = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificatioStatus = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccountNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    ServiceProviding = table.Column<int>(type: "int", nullable: false),
                    ServiceProviderRole = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CarWashers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarWashers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CarWashings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CustomerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CarWasherId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CarBrand = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CarModel = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ServiceStatus = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CarWashings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarWashings_CarWashers_CarWasherId",
                        column: x => x.CarWasherId,
                        principalTable: "CarWashers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CarWashings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CarWashItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CarWasherId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
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
                    table.PrimaryKey("PK_CarWashItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarWashItems_CarWashers_CarWasherId",
                        column: x => x.CarWasherId,
                        principalTable: "CarWashers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CarWashNegotiations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CarWashingId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CarWasherId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NegotiatedPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
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
                    table.PrimaryKey("PK_CarWashNegotiations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarWashNegotiations_CarWashers_CarWasherId",
                        column: x => x.CarWasherId,
                        principalTable: "CarWashers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarWashNegotiations_CarWashings_CarWashingId",
                        column: x => x.CarWashingId,
                        principalTable: "CarWashings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CarWashingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CarWashingId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CarWashItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
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
                    table.PrimaryKey("PK_CarWashingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarWashingItems_CarWashItems_CarWashItemId",
                        column: x => x.CarWashItemId,
                        principalTable: "CarWashItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarWashingItems_CarWashings_CarWashingId",
                        column: x => x.CarWashingId,
                        principalTable: "CarWashings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashers_UserId",
                table: "CarWashers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashingItems_CarWashingId",
                table: "CarWashingItems",
                column: "CarWashingId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashingItems_CarWashItemId",
                table: "CarWashingItems",
                column: "CarWashItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashings_CarWasherId",
                table: "CarWashings",
                column: "CarWasherId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashings_CustomerId",
                table: "CarWashings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashItems_CarWasherId",
                table: "CarWashItems",
                column: "CarWasherId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashNegotiations_CarWasherId",
                table: "CarWashNegotiations",
                column: "CarWasherId");

            migrationBuilder.CreateIndex(
                name: "IX_CarWashNegotiations_CarWashingId",
                table: "CarWashNegotiations",
                column: "CarWashingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarWashingItems");

            migrationBuilder.DropTable(
                name: "CarWashNegotiations");

            migrationBuilder.DropTable(
                name: "CarWashItems");

            migrationBuilder.DropTable(
                name: "CarWashings");

            migrationBuilder.DropTable(
                name: "CarWashers");

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "TransactionId",
                keyValue: null,
                column: "TransactionId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
