using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumVitaeApp.Migrations
{
    /// <inheritdoc />
    public partial class ClaveForaneaCurriculumIDCurriculumSeleccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CurriculumSeleccion_CurriculumID",
                table: "CurriculumSeleccion",
                column: "CurriculumID");

            migrationBuilder.AddForeignKey(
                name: "FK_CurriculumSeleccion_Curriculum_CurriculumID",
                table: "CurriculumSeleccion",
                column: "CurriculumID",
                principalTable: "Curriculum",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurriculumSeleccion_Curriculum_CurriculumID",
                table: "CurriculumSeleccion");

            migrationBuilder.DropIndex(
                name: "IX_CurriculumSeleccion_CurriculumID",
                table: "CurriculumSeleccion");
        }
    }
}
