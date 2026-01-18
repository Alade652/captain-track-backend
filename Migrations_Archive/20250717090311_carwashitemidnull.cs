using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaptainTrackBackend.Application.Migrations
{
    /// <inheritdoc />
    public partial class carwashitemidnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarWashItems_CarWashers_CarWasherId",
                table: "CarWashItems");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedFor",
                table: "Ratings",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "CarWasherId",
                table: "CarWashItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_CarWashItems_CarWashers_CarWasherId",
                table: "CarWashItems",
                column: "CarWasherId",
                principalTable: "CarWashers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarWashItems_CarWashers_CarWasherId",
                table: "CarWashItems");

            migrationBuilder.DropColumn(
                name: "CreatedFor",
                table: "Ratings");

            migrationBuilder.AlterColumn<Guid>(
                name: "CarWasherId",
                table: "CarWashItems",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_CarWashItems_CarWashers_CarWasherId",
                table: "CarWashItems",
                column: "CarWasherId",
                principalTable: "CarWashers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
