using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIDCurriculum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Curriculum");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioID",
                table: "Curriculum",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Curriculum_UsuarioID",
                table: "Curriculum",
                column: "UsuarioID");

            migrationBuilder.AddForeignKey(
                name: "FK_Curriculum_Usuarios_UsuarioID",
                table: "Curriculum",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curriculum_Usuarios_UsuarioID",
                table: "Curriculum");

            migrationBuilder.DropIndex(
                name: "IX_Curriculum_UsuarioID",
                table: "Curriculum");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "Curriculum");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Curriculum",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
