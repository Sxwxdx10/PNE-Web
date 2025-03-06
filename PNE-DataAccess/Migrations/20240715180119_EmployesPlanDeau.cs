using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EmployesPlanDeau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployePlaneaus",
                columns: table => new
                {
                    IdEmployePlaneau = table.Column<string>(type: "text", nullable: false),
                    IdUtilisateur = table.Column<string>(type: "character varying", nullable: false),
                    IdPlaneau = table.Column<string>(type: "character varying(10)", nullable: false),
                    EstGerant = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployePlaneaus", x => x.IdEmployePlaneau);
                    table.ForeignKey(
                        name: "FK_EmployePlaneau_IdPlaneau",
                        column: x => x.IdPlaneau,
                        principalTable: "planeau",
                        principalColumn: "id_plan_eau",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployePlaneau_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployePlaneaus_IdPlaneau",
                table: "EmployePlaneaus",
                column: "IdPlaneau");

            migrationBuilder.CreateIndex(
                name: "IX_EmployePlaneaus_IdUtilisateur",
                table: "EmployePlaneaus",
                column: "IdUtilisateur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployePlaneaus");
        }
    }
}
