using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNombreJoursSejourToMiseaeau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "codeQR",
                table: "embarcation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codeQR",
                table: "embarcation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
