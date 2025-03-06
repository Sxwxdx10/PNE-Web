using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "nom_role", "Description" },
                values: new object[,]
                {
                    { "admin", "Administarteur, employe de la CREE" },
                    { "chercheur", "personne qui fait des recherche sur la propagation des EEE" },
                    { "employe", "personne qui travail a un plan d'eau" },
                    { "gerant", "le gerant d'un plan d'eau" },
                    { "patrouilleur", "Membre des forces de l'ordre" },
                    { "plaisancier", "personne qui aime bien les bateaux" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "admin");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "chercheur");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "employe");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "gerant");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "patrouilleur");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "nom_role",
                keyValue: "plaisancier");
        }
    }
}
