using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ConvertEnumsToStrings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "station_personnel_status",
                table: "StationLavages",
                type: "character varying",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "station_personnel_status");

            migrationBuilder.AlterColumn<string>(
                name: "niveau_couleur",
                table: "planeau",
                type: "character varying",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "niveau",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "type_lavage",
                table: "lavage",
                type: "character varying",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "type_lavage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "station_personnel_status",
                table: "StationLavages",
                type: "station_personnel_status",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.AlterColumn<int>(
                name: "niveau_couleur",
                table: "planeau",
                type: "niveau",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "type_lavage",
                table: "lavage",
                type: "type_lavage",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying");
        }
    }
}
