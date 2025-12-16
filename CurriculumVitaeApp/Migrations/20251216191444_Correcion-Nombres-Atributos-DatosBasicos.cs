using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class CorrecionNombresAtributosDatosBasicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "DatosBasicos",
                newName: "ValorDato");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "DatosBasicos",
                newName: "NombreDato");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValorDato",
                table: "DatosBasicos",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "NombreDato",
                table: "DatosBasicos",
                newName: "Nombre");
        }
    }
}
