using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace beautysalon.Migrations
{
    /// <inheritdoc />
    public partial class updatedCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "Companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "Companies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
