using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecetaToProductoStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecetaId",
                table: "ProductosStock",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductosStock_RecetaId",
                table: "ProductosStock",
                column: "RecetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductosStock_Recetas_RecetaId",
                table: "ProductosStock",
                column: "RecetaId",
                principalTable: "Recetas",
                principalColumn: "IdReceta",
                onDelete: ReferentialAction.SetNull);

            // Migración de datos: inferir RecetaId de los movimientos existentes
            // Solo actualiza entradas donde TODOS los movimientos de producción
            // provienen de la misma receta (caso único → asignación segura)
            migrationBuilder.Sql(@"
                UPDATE ps
                SET ps.RecetaId = (
                    SELECT DISTINCT l.RecetaId
                    FROM MovimientosStock ms
                    JOIN Lotes l ON ms.LoteId = l.Id
                    WHERE ms.ProductoStockId = ps.Id
                      AND ms.MotivoMovimiento = 'Produccion'
                )
                FROM ProductosStock ps
                WHERE ps.RecetaId IS NULL
                  AND (
                    SELECT COUNT(DISTINCT l2.RecetaId)
                    FROM MovimientosStock ms2
                    JOIN Lotes l2 ON ms2.LoteId = l2.Id
                    WHERE ms2.ProductoStockId = ps.Id
                      AND ms2.MotivoMovimiento = 'Produccion'
                  ) = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductosStock_Recetas_RecetaId",
                table: "ProductosStock");

            migrationBuilder.DropIndex(
                name: "IX_ProductosStock_RecetaId",
                table: "ProductosStock");

            migrationBuilder.DropColumn(
                name: "RecetaId",
                table: "ProductosStock");
        }
    }
}
