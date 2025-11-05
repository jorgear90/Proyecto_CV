using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class CrearBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tipoInstitucion",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipoInstitucion", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExperienciaLaborals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaTermino = table.Column<DateOnly>(type: "date", nullable: true),
                    Vigente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienciaLaborals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperienciaLaborals_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormacionAcademica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    TipoInstitucionID = table.Column<int>(type: "int", nullable: false),
                    Carrera = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnhoInicio = table.Column<int>(type: "int", nullable: false),
                    AnhoTermino = table.Column<int>(type: "int", nullable: true),
                    Vigente = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormacionAcademica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormacionAcademica_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormacionAcademica_tipoInstitucion_TipoInstitucionID",
                        column: x => x.TipoInstitucionID,
                        principalTable: "tipoInstitucion",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Habilidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habilidades_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Perfil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfil", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Perfil_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciaLaborals_UsuarioID",
                table: "ExperienciaLaborals",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_FormacionAcademica_TipoInstitucionID",
                table: "FormacionAcademica",
                column: "TipoInstitucionID");

            migrationBuilder.CreateIndex(
                name: "IX_FormacionAcademica_UsuarioID",
                table: "FormacionAcademica",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Habilidades_UsuarioID",
                table: "Habilidades",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Perfil_UsuarioID",
                table: "Perfil",
                column: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExperienciaLaborals");

            migrationBuilder.DropTable(
                name: "FormacionAcademica");

            migrationBuilder.DropTable(
                name: "Habilidades");

            migrationBuilder.DropTable(
                name: "Perfil");

            migrationBuilder.DropTable(
                name: "tipoInstitucion");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
