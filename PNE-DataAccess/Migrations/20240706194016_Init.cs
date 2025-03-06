using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the enums
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    CREATE TYPE niveau AS ENUM ('vert', 'jaune', 'rouge');
                    CREATE TYPE type_lavage AS ENUM ('eau_chaude_avec_pression', 'eau_froide_avec_pression', 'eau_chaude_sans_pression', 'eau_froide_sans_pression');
                    CREATE TYPE type_pne_id AS ENUM ('serial_embarcation', 'serial_lavage', 'serial_embarcation_utilisateur', 'serial_note', 'serial_plan_eau', 'serial_mise_eau');
                EXCEPTION
                    WHEN duplicate_object THEN null;
                END $$;
            ");

            migrationBuilder.CreateSequence(
                name: "serial_embarcation");

            migrationBuilder.CreateSequence(
                name: "serial_embarcation_utilisateur");

            migrationBuilder.CreateSequence(
                name: "serial_lavage");

            migrationBuilder.CreateSequence(
                name: "serial_mise_eau");

            migrationBuilder.CreateSequence(
                name: "serial_note");

            migrationBuilder.CreateSequence(
                name: "serial_plan_eau");

            migrationBuilder.CreateTable(
                name: "certification",
                columns: table => new
                {
                    code_certification = table.Column<string>(type: "character varying", nullable: false),
                    nom_formation = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("certification_pkey", x => x.code_certification);
                });

            migrationBuilder.CreateTable(
                name: "embarcation",
                columns: table => new
                {
                    id_embarcation = table.Column<string>(type: "character varying", nullable: false),
                    description = table.Column<string>(type: "character varying", nullable: false),
                    marque = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    longueur = table.Column<int>(type: "integer", nullable: false),
                    photo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    codeQR = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("embarcation_pkey", x => x.id_embarcation);
                });

            migrationBuilder.CreateTable(
                name: "planeau",
                columns: table => new
                {
                    id_plan_eau = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    emplacement = table.Column<Point>(type: "geometry(Point)", nullable: false),
                    nom = table.Column<string>(type: "character varying", nullable: false),
                    niveau_couleur = table.Column<int>(type: "niveau", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("planeau_pkey", x => x.id_plan_eau);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    nom_role = table.Column<string>(type: "character varying", nullable: false),
                    Description = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("role_pkey", x => x.nom_role);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying", nullable: false),
                    Email = table.Column<string>(type: "character varying", nullable: false),
                    display_name = table.Column<string>(type: "character varying", nullable: false),
                    date_creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lavage",
                columns: table => new
                {
                    id_lavage = table.Column<string>(type: "character varying", maxLength: 10, nullable: false),
                    id_embarcation = table.Column<string>(type: "character varying", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    self_serve = table.Column<bool>(type: "boolean", nullable: false),
                    type_lavage = table.Column<int>(type: "type_lavage", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lavage_pkey", x => x.id_lavage);
                    table.ForeignKey(
                        name: "lavage_id_embarcation_fkey",
                        column: x => x.id_embarcation,
                        principalTable: "embarcation",
                        principalColumn: "id_embarcation");
                });

            migrationBuilder.CreateTable(
                name: "miseaeau",
                columns: table => new
                {
                    id_mise_eau = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_plan_eau = table.Column<string>(type: "character varying(10)", nullable: true),
                    id_embarcation = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("miseaeau_pkey", x => x.id_mise_eau);
                    table.ForeignKey(
                        name: "miseaeau_id_embarcation_utilisateur_fkey",
                        column: x => x.id_embarcation,
                        principalTable: "embarcation",
                        principalColumn: "id_embarcation",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "miseaeau_id_plan_eau_fkey",
                        column: x => x.id_plan_eau,
                        principalTable: "planeau",
                        principalColumn: "id_plan_eau");
                });

            migrationBuilder.CreateTable(
                name: "notedossier",
                columns: table => new
                {
                    idnote = table.Column<string>(type: "text", nullable: false),
                    id_embarcation = table.Column<string>(type: "character varying", nullable: true),
                    id_plan_eau = table.Column<string>(type: "character varying(10)", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notedossier_pkey", x => x.idnote);
                    table.ForeignKey(
                        name: "notedossier_id_embarcation_utilisateur_fkey",
                        column: x => x.id_embarcation,
                        principalTable: "embarcation",
                        principalColumn: "id_embarcation");
                    table.ForeignKey(
                        name: "notedossier_id_plan_eau_fkey",
                        column: x => x.id_plan_eau,
                        principalTable: "planeau",
                        principalColumn: "id_plan_eau");
                });

            migrationBuilder.CreateTable(
                name: "CertificationUtilisateurs",
                columns: table => new
                {
                    IdCertificationUtilisateur = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeCertification = table.Column<string>(type: "character varying", nullable: false),
                    IdUtilisateur = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationUtilisateurs", x => x.IdCertificationUtilisateur);
                    table.ForeignKey(
                        name: "FK_CertificationUtilisateur_CodeCertification",
                        column: x => x.CodeCertification,
                        principalTable: "certification",
                        principalColumn: "code_certification",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificationUtilisateur_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Embarcationutilisateurs",
                columns: table => new
                {
                    id_embarcation_utilisateur = table.Column<string>(type: "text", nullable: false),
                    id_embarcation = table.Column<string>(type: "character varying", nullable: true),
                    id_utilisateur = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embarcationutilisateurs", x => x.id_embarcation_utilisateur);
                    table.ForeignKey(
                        name: "embarcationutilisateur_id_embarcation_fkey",
                        column: x => x.id_embarcation,
                        principalTable: "embarcation",
                        principalColumn: "id_embarcation");
                    table.ForeignKey(
                        name: "embarcationutilisateur_sub_fkey",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "RolesUtilisateurs",
                columns: table => new
                {
                    IdRolesUtilisateurs = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_role = table.Column<string>(type: "character varying", nullable: false),
                    IdUtilisateur = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesUtilisateurs", x => x.IdRolesUtilisateurs);
                    table.ForeignKey(
                        name: "FK_RolesUtilisateurs_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolesUtilisateurs_NomRole",
                        column: x => x.nom_role,
                        principalTable: "role",
                        principalColumn: "nom_role",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificationUtilisateurs_CodeCertification",
                table: "CertificationUtilisateurs",
                column: "CodeCertification");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationUtilisateurs_IdUtilisateur",
                table: "CertificationUtilisateurs",
                column: "IdUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Embarcationutilisateurs_id_embarcation",
                table: "Embarcationutilisateurs",
                column: "id_embarcation");

            migrationBuilder.CreateIndex(
                name: "IX_Embarcationutilisateurs_id_utilisateur",
                table: "Embarcationutilisateurs",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_lavage_id_embarcation",
                table: "lavage",
                column: "id_embarcation");

            migrationBuilder.CreateIndex(
                name: "IX_miseaeau_id_embarcation",
                table: "miseaeau",
                column: "id_embarcation");

            migrationBuilder.CreateIndex(
                name: "IX_miseaeau_id_plan_eau",
                table: "miseaeau",
                column: "id_plan_eau");

            migrationBuilder.CreateIndex(
                name: "IX_notedossier_id_embarcation",
                table: "notedossier",
                column: "id_embarcation");

            migrationBuilder.CreateIndex(
                name: "IX_notedossier_id_plan_eau",
                table: "notedossier",
                column: "id_plan_eau");

            migrationBuilder.CreateIndex(
                name: "IX_RolesUtilisateurs_IdUtilisateur",
                table: "RolesUtilisateurs",
                column: "IdUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_RolesUtilisateurs_nom_role",
                table: "RolesUtilisateurs",
                column: "nom_role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DROP TYPE IF EXISTS niveau;");
            migrationBuilder.Sql("DROP TYPE IF EXISTS type_lavage;");
            migrationBuilder.Sql("DROP TYPE IF EXISTS type_pne_id;");

            migrationBuilder.DropTable(
                name: "CertificationUtilisateurs");

            migrationBuilder.DropTable(
                name: "Embarcationutilisateurs");

            migrationBuilder.DropTable(
                name: "lavage");

            migrationBuilder.DropTable(
                name: "miseaeau");

            migrationBuilder.DropTable(
                name: "notedossier");

            migrationBuilder.DropTable(
                name: "RolesUtilisateurs");

            migrationBuilder.DropTable(
                name: "certification");

            migrationBuilder.DropTable(
                name: "embarcation");

            migrationBuilder.DropTable(
                name: "planeau");

            migrationBuilder.DropTable(
                name: "Utilisateur");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropSequence(
                name: "serial_embarcation");

            migrationBuilder.DropSequence(
                name: "serial_embarcation_utilisateur");

            migrationBuilder.DropSequence(
                name: "serial_lavage");

            migrationBuilder.DropSequence(
                name: "serial_mise_eau");

            migrationBuilder.DropSequence(
                name: "serial_note");

            migrationBuilder.DropSequence(
                name: "serial_plan_eau");
        }
    }
}
