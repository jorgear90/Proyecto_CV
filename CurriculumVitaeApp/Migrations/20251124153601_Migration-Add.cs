using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class MigrationAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperienciaLaborals_Usuarios_UsuarioID",
                table: "ExperienciaLaborals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperienciaLaborals",
                table: "ExperienciaLaborals");

            migrationBuilder.RenameTable(
                name: "ExperienciaLaborals",
                newName: "AntecedentesLaborales");

            migrationBuilder.RenameIndex(
                name: "IX_ExperienciaLaborals_UsuarioID",
                table: "AntecedentesLaborales",
                newName: "IX_AntecedentesLaborales_UsuarioID");

            migrationBuilder.AlterColumn<bool>(
                name: "Vigente",
                table: "AntecedentesLaborales",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AntecedentesLaborales",
                table: "AntecedentesLaborales",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AntecedentesLaborales_Usuarios_UsuarioID",
                table: "AntecedentesLaborales",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AntecedentesLaborales_Usuarios_UsuarioID",
                table: "AntecedentesLaborales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AntecedentesLaborales",
                table: "AntecedentesLaborales");

            migrationBuilder.RenameTable(
                name: "AntecedentesLaborales",
                newName: "ExperienciaLaborals");

            migrationBuilder.RenameIndex(
                name: "IX_AntecedentesLaborales_UsuarioID",
                table: "ExperienciaLaborals",
                newName: "IX_ExperienciaLaborals_UsuarioID");

            migrationBuilder.AlterColumn<string>(
                name: "Vigente",
                table: "ExperienciaLaborals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperienciaLaborals",
                table: "ExperienciaLaborals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExperienciaLaborals_Usuarios_UsuarioID",
                table: "ExperienciaLaborals",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
