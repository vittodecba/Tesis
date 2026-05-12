using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    public partial class UpdatePedidoProductoStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM DetallesPedidos");
            migrationBuilder.Sql("DELETE FROM Pedidos");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('DetallesPedidos', 'ProductoId') IS NOT NULL 
                EXEC sp_rename 'DetallesPedidos.ProductoId', 'ProductoStockId', 'COLUMN'
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedidos_ProductosStock_ProductoStockId",
                table: "DetallesPedidos",
                column: "ProductoStockId",
                principalTable: "ProductosStock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}