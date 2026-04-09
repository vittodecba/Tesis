using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLotePrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar FKs duplicadas/residuales de LotePrueba
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_Lotes_LoteId1",
                table: "RegistrosFermentacion");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_LotesPrueba_LoteId",
                table: "RegistrosFermentacion");

            // Eliminar tablas de prueba si existen
            migrationBuilder.DropTable(
                name: "LotesPrueba");

            migrationBuilder.DropTable(
                name: "FermentadoresPruebas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No se recrea lo de prueba en el rollback
        }
    }
}