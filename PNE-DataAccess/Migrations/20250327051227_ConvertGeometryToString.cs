using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ConvertGeometryToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "position",
                table: "StationLavages",
                newName: "Position");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "StationLavages",
                type: "character varying",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Point),
                oldType: "geometry(Point)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "emplacement",
                table: "planeau",
                type: "character varying",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Point),
                oldType: "geometry(Point)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "StationLavages",
                newName: "position");

            migrationBuilder.AlterColumn<Point>(
                name: "position",
                table: "StationLavages",
                type: "geometry(Point)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.AlterColumn<Point>(
                name: "emplacement",
                table: "planeau",
                type: "geometry(Point)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying");
        }
    }
}
