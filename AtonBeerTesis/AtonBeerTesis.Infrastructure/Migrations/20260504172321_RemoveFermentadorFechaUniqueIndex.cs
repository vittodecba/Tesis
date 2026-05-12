using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFermentadorFechaUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion");

            migrationBuilder.CreateIndex(
                name: "IX_PlanificacionProduccion_FermentadorId",
                table: "PlanificacionProduccion",
                column: "FermentadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlanificacionProduccion_FermentadorId",
                table: "PlanificacionProduccion");

            migrationBuilder.CreateIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion",
                columns: new[] { "FermentadorId", "FechaInicio" },
                unique: true,
                filter: "[FermentadorId] IS NOT NULL");
        }
    }
}
