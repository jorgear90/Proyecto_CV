using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class EncabezadoNombreCurriculum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Encabezado",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Encabezado",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Curriculum");
        }
    }
}
