using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaTipoDatoCurriculum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDato",
                table: "CurriculumSeleccion");

            migrationBuilder.AddColumn<int>(
                name: "TipoDatoCurriculumID",
                table: "CurriculumSeleccion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TipoDatoCurriculum",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipoDato = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDatoCurriculum", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSeleccion_TipoDatoCurriculumID",
                table: "CurriculumSeleccion",
                column: "TipoDatoCurriculumID");

            migrationBuilder.AddForeignKey(
                name: "FK_CurriculumSeleccion_TipoDatoCurriculum_TipoDatoCurriculumID",
                table: "CurriculumSeleccion",
                column: "TipoDatoCurriculumID",
                principalTable: "TipoDatoCurriculum",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurriculumSeleccion_TipoDatoCurriculum_TipoDatoCurriculumID",
                table: "CurriculumSeleccion");

            migrationBuilder.DropTable(
                name: "TipoDatoCurriculum");

            migrationBuilder.DropIndex(
                name: "IX_CurriculumSeleccion_TipoDatoCurriculumID",
                table: "CurriculumSeleccion");

            migrationBuilder.DropColumn(
                name: "TipoDatoCurriculumID",
                table: "CurriculumSeleccion");

            migrationBuilder.AddColumn<string>(
                name: "TipoDato",
                table: "CurriculumSeleccion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
