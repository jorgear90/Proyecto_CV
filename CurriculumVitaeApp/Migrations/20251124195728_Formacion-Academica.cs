using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class FormacionAcademica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Vigente",
                table: "FormacionAcademica",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "FormacionAcademica",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreInstitucion",
                table: "FormacionAcademica",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "FormacionAcademica");

            migrationBuilder.DropColumn(
                name: "NombreInstitucion",
                table: "FormacionAcademica");

            migrationBuilder.AlterColumn<string>(
                name: "Vigente",
                table: "FormacionAcademica",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
