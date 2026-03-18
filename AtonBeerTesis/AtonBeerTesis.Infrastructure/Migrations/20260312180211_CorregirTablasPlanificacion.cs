using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorregirTablasPlanificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanificacionProduccion_FermentadoresPruebas_FermentadorId",
                table: "PlanificacionProduccion");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "PlanificacionProduccion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "PlanificacionProduccion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId",
                table: "PlanificacionProduccion",
                column: "FermentadorId",
                principalTable: "Fermentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId",
                table: "PlanificacionProduccion");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "PlanificacionProduccion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "PlanificacionProduccion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanificacionProduccion_FermentadoresPruebas_FermentadorId",
                table: "PlanificacionProduccion",
                column: "FermentadorId",
                principalTable: "FermentadoresPruebas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
