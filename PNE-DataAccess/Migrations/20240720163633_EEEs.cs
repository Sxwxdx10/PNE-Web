using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EEEs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "miseaeau_id_embarcation_utilisateur_fkey",
                table: "miseaeau");

            migrationBuilder.DropForeignKey(
                name: "miseaeau_id_plan_eau_fkey",
                table: "miseaeau");

            migrationBuilder.CreateTable(
                name: "EEEEs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    NiveauCouleur = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EEEEs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EEEPlanEau",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IdEEE = table.Column<string>(type: "text", nullable: false),
                    IdPlanEau = table.Column<string>(type: "character varying(10)", nullable: false),
                    Validated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EEEPlanEau", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EEEPlanEau_IdEEE",
                        column: x => x.IdEEE,
                        principalTable: "EEEEs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EEEPlanEau_IdPlanEau",
                        column: x => x.IdPlanEau,
                        principalTable: "planeau",
                        principalColumn: "id_plan_eau",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EEEPlanEau_IdEEE",
                table: "EEEPlanEau",
                column: "IdEEE");

            migrationBuilder.CreateIndex(
                name: "IX_EEEPlanEau_IdPlanEau",
                table: "EEEPlanEau",
                column: "IdPlanEau");

            migrationBuilder.AddForeignKey(
                name: "FK_miseaeau_embarcation_id_embarcation",
                table: "miseaeau",
                column: "id_embarcation",
                principalTable: "embarcation",
                principalColumn: "id_embarcation",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_miseaeau_planeau_id_plan_eau",
                table: "miseaeau",
                column: "id_plan_eau",
                principalTable: "planeau",
                principalColumn: "id_plan_eau");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_miseaeau_embarcation_id_embarcation",
                table: "miseaeau");

            migrationBuilder.DropForeignKey(
                name: "FK_miseaeau_planeau_id_plan_eau",
                table: "miseaeau");

            migrationBuilder.DropTable(
                name: "EEEPlanEau");

            migrationBuilder.DropTable(
                name: "EEEEs");

            migrationBuilder.AddForeignKey(
                name: "miseaeau_id_embarcation_utilisateur_fkey",
                table: "miseaeau",
                column: "id_embarcation",
                principalTable: "embarcation",
                principalColumn: "id_embarcation",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "miseaeau_id_plan_eau_fkey",
                table: "miseaeau",
                column: "id_plan_eau",
                principalTable: "planeau",
                principalColumn: "id_plan_eau");
        }
    }
}
