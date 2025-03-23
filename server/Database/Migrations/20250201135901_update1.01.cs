using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace beautysalon.Database.Migrations
{
    /// <inheritdoc />
    public partial class update101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyID",
                table: "Services",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Services_CompanyID",
                table: "Services",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Companies_CompanyID",
                table: "Services",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Companies_CompanyID",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_CompanyID",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "Services");
        }
    }
}
