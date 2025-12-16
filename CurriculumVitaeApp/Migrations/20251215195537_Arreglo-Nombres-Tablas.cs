using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class ArregloNombresTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AntecedentesLaborales_Usuarios_UsuarioID",
                table: "AntecedentesLaborales");

            migrationBuilder.DropForeignKey(
                name: "FK_FormacionAcademica_tipoInstitucion_TipoInstitucionID",
                table: "FormacionAcademica");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfil_Usuarios_UsuarioID",
                table: "Perfil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tipoInstitucion",
                table: "tipoInstitucion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfil",
                table: "Perfil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AntecedentesLaborales",
                table: "AntecedentesLaborales");

            migrationBuilder.RenameTable(
                name: "tipoInstitucion",
                newName: "TipoInstitucion");

            migrationBuilder.RenameTable(
                name: "Perfil",
                newName: "DatosBasicos");

            migrationBuilder.RenameTable(
                name: "AntecedentesLaborales",
                newName: "ExperienciaLaboral");

            migrationBuilder.RenameIndex(
                name: "IX_Perfil_UsuarioID",
                table: "DatosBasicos",
                newName: "IX_DatosBasicos_UsuarioID");

            migrationBuilder.RenameIndex(
                name: "IX_AntecedentesLaborales_UsuarioID",
                table: "ExperienciaLaboral",
                newName: "IX_ExperienciaLaboral_UsuarioID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoInstitucion",
                table: "TipoInstitucion",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatosBasicos",
                table: "DatosBasicos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperienciaLaboral",
                table: "ExperienciaLaboral",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DatosBasicos_Usuarios_UsuarioID",
                table: "DatosBasicos",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExperienciaLaboral_Usuarios_UsuarioID",
                table: "ExperienciaLaboral",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormacionAcademica_TipoInstitucion_TipoInstitucionID",
                table: "FormacionAcademica",
                column: "TipoInstitucionID",
                principalTable: "TipoInstitucion",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatosBasicos_Usuarios_UsuarioID",
                table: "DatosBasicos");

            migrationBuilder.DropForeignKey(
                name: "FK_ExperienciaLaboral_Usuarios_UsuarioID",
                table: "ExperienciaLaboral");

            migrationBuilder.DropForeignKey(
                name: "FK_FormacionAcademica_TipoInstitucion_TipoInstitucionID",
                table: "FormacionAcademica");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoInstitucion",
                table: "TipoInstitucion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperienciaLaboral",
                table: "ExperienciaLaboral");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DatosBasicos",
                table: "DatosBasicos");

            migrationBuilder.RenameTable(
                name: "TipoInstitucion",
                newName: "tipoInstitucion");

            migrationBuilder.RenameTable(
                name: "ExperienciaLaboral",
                newName: "AntecedentesLaborales");

            migrationBuilder.RenameTable(
                name: "DatosBasicos",
                newName: "Perfil");

            migrationBuilder.RenameIndex(
                name: "IX_ExperienciaLaboral_UsuarioID",
                table: "AntecedentesLaborales",
                newName: "IX_AntecedentesLaborales_UsuarioID");

            migrationBuilder.RenameIndex(
                name: "IX_DatosBasicos_UsuarioID",
                table: "Perfil",
                newName: "IX_Perfil_UsuarioID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tipoInstitucion",
                table: "tipoInstitucion",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AntecedentesLaborales",
                table: "AntecedentesLaborales",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfil",
                table: "Perfil",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AntecedentesLaborales_Usuarios_UsuarioID",
                table: "AntecedentesLaborales",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormacionAcademica_tipoInstitucion_TipoInstitucionID",
                table: "FormacionAcademica",
                column: "TipoInstitucionID",
                principalTable: "tipoInstitucion",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfil_Usuarios_UsuarioID",
                table: "Perfil",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
