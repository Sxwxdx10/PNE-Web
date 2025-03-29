using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using PNE_core.Enums;

#nullable disable

namespace PNE_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCoursEauEstrie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
    name: "emplacement",
    table: "planeau",
    type: "geometry(Point)",
    nullable: true,
    oldClrType: typeof(Point),
    oldType: "geometry(Point)");

            migrationBuilder.InsertData(
       table: "planeau",
       columns: new[] { "id_plan_eau", "nom", "niveau_couleur", "emplacement" },
       values: new object[,]
       {
            { "CE001", "Rivière au Canard", "'vert'", null },
            { "CE002", "Ruisseau Caouette", "'vert'", null },
            { "CE003", "Rivière aux Cerises", "'vert'", null },
            { "CE004", "Rivière Chaudière", "'vert'", null },
            { "CE005", "Rivière Chesham", "'vert'", null },
            { "CE006", "Rivière Clifton", "'vert'", null },
            { "CE007", "Rivière Clinton", "'vert'", null },
            { "CE008", "Rivière Coaticook", "'vert'", null },
            { "CE009", "Rivière Danville", "'vert'", null },
            { "CE010", "Rivière Ditton Est", "'vert'", null },
            { "CE011", "Rivière Ditton Ouest", "'vert'", null },
            { "CE012", "Rivière Ditton", "'vert'", null },
            { "CE013", "Rivière Drolet", "'vert'", null },
            { "CE014", "Rivière Eaton Nord", "'vert'", null },
            { "CE015", "Rivière Eaton", "'vert'", null },
            { "CE016", "Rivière Felton", "'vert'", null },
            { "CE017", "Rivière Fraser", "'vert'", null },
            { "CE018", "Rivière Glen", "'vert'", null },
            { "CE019", "Rivière Magog", "'vert'", null },
            { "CE020", "Rivière Saint-François", "'vert'", null },
            { "CE021", "Rivière Weedon", "'vert'", null },
            { "CE022", "Rivière au Saumon", "'vert'", null },
       
       });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

                migrationBuilder.AlterColumn<Point>(
        name: "emplacement",
        table: "planeau",
        type: "geometry(Point)",
        nullable: true,
        oldClrType: typeof(Point),
        oldType: "geometry(Point)");

            migrationBuilder.DeleteData(
    table: "Planeau",
    keyColumn: "IdPlanEau",
    keyValues: new object[]
    {
            "CE001", "CE002", "CE003", "CE004", "CE005",
            "CE006", "CE007", "CE008", "CE009", "CE010",
            "CE011", "CE012", "CE013", "CE014", "CE015",
            "CE016", "CE017", "CE018", "CE019", "CE020",
            "CE021", "CE022",
      
    });
        }
    }
}
