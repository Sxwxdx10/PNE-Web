using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSignaleurToEEE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdSignaleur",
                table: "EEEEs",
                type: "text",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_EEEEs_IdSignaleur",
                table: "EEEEs",
                column: "IdSignaleur");

            migrationBuilder.AddForeignKey(
                name: "FK_EEE_IdSignaleur",
                table: "EEEEs",
                column: "IdSignaleur",
                principalTable: "Utilisateur",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EEE_IdSignaleur",
                table: "EEEEs");

            migrationBuilder.DropIndex(
                name: "IX_EEEEs_IdSignaleur",
                table: "EEEEs");

            migrationBuilder.DropColumn(
                name: "IdSignaleur",
                table: "EEEEs");
        }
    }
}
