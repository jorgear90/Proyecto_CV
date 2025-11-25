using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Perfil",
                newName: "Valor");

            migrationBuilder.AlterColumn<bool>(
                name: "Vigente",
                table: "FormacionAcademica",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Perfil",
                newName: "Descripcion");

            migrationBuilder.AlterColumn<bool>(
                name: "Vigente",
                table: "FormacionAcademica",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
