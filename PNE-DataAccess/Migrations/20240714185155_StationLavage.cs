using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class StationLavage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    CREATE TYPE station_personnel_status AS ENUM ('aucun','present','certifie_decontamination');
                EXCEPTION
                    WHEN duplicate_object THEN null;
                END $$;
            ");


            migrationBuilder.CreateTable(
                name: "StationLavages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying", nullable: false),
                    nom = table.Column<string>(type: "character varying", nullable: false),
                    position = table.Column<Point>(type: "geometry(Point)", nullable: false),
                    planeauIdPlanEau = table.Column<string>(type: "character varying(10)", nullable: true),
                    PeutDecontaminer = table.Column<bool>(type: "boolean", nullable: true),
                    HautePression = table.Column<bool>(type: "boolean", nullable: true),
                    BassePressionetAttaches = table.Column<bool>(type: "boolean", nullable: true),
                    EauChaude = table.Column<bool>(type: "boolean", nullable: true),
                    station_personnel_status = table.Column<int>(type: "station_personnel_status", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLavages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationLavages_planeau_planeauIdPlanEau",
                        column: x => x.planeauIdPlanEau,
                        principalTable: "planeau",
                        principalColumn: "id_plan_eau");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StationLavages_planeauIdPlanEau",
                table: "StationLavages",
                column: "planeauIdPlanEau");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationLavages");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:niveau.niveau", "vert,jaune,rouge")
                .Annotation("Npgsql:Enum:type_lavage.type_lavage", "eau_chaude_avec_pression,eau_froide_avec_pression,eau_chaude_sans_pression,eau_froide_sans_pression")
                .Annotation("Npgsql:Enum:type_pne_id.type_pne_id", "serial_embarcation,serial_lavage,serial_embarcation_utilisateur,serial_note,serial_plan_eau,serial_mise_eau")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:niveau.niveau", "vert,jaune,rouge")
                .OldAnnotation("Npgsql:Enum:station_personnel_status.station_personnel_status", "aucun,present,certifie_decontamination")
                .OldAnnotation("Npgsql:Enum:type_lavage.type_lavage", "eau_chaude_avec_pression,eau_froide_avec_pression,eau_chaude_sans_pression,eau_froide_sans_pression")
                .OldAnnotation("Npgsql:Enum:type_pne_id.type_pne_id", "serial_embarcation,serial_lavage,serial_embarcation_utilisateur,serial_note,serial_plan_eau,serial_mise_eau")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
