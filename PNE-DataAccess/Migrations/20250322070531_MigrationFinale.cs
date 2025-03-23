using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MigrationFinale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "nom_role", "Description" },
                values: new object[,]
                {
                    { "superadmin", "Administrateur global du système avec tous les droits" },
                    { "supergerant", "Gérant responsable de plusieurs plans d'eau" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "superadmin");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "supergerant");
        }
    }
}
