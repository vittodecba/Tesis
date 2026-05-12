using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StockRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lote",
                table: "MovimientosStock");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "MovimientosStock",
                newName: "ProductoStockId");

            migrationBuilder.AddColumn<int>(
                name: "LoteId",
                table: "MovimientosStock",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormatosEnvase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CapacidadLitros = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatosEnvase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoteDesignaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoteId = table.Column<int>(type: "int", nullable: false),
                    FormatoEnvaseId = table.Column<int>(type: "int", nullable: false),
                    VolumenAsignado = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoteDesignaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoteDesignaciones_FormatosEnvase_FormatoEnvaseId",
                        column: x => x.FormatoEnvaseId,
                        principalTable: "FormatosEnvase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoteDesignaciones_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductosStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatoEnvaseId = table.Column<int>(type: "int", nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductosStock_FormatosEnvase_FormatoEnvaseId",
                        column: x => x.FormatoEnvaseId,
                        principalTable: "FormatosEnvase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Limpiar movimientos viejos que referencian ProductosPrueba (datos de desarrollo)
            migrationBuilder.Sql("DELETE FROM MovimientosStock");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_ProductoStockId",
                table: "MovimientosStock",
                column: "ProductoStockId");

            migrationBuilder.CreateIndex(
                name: "IX_LoteDesignaciones_FormatoEnvaseId",
                table: "LoteDesignaciones",
                column: "FormatoEnvaseId");

            migrationBuilder.CreateIndex(
                name: "IX_LoteDesignaciones_LoteId",
                table: "LoteDesignaciones",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosStock_FormatoEnvaseId",
                table: "ProductosStock",
                column: "FormatoEnvaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosStock_ProductosStock_ProductoStockId",
                table: "MovimientosStock",
                column: "ProductoStockId",
                principalTable: "ProductosStock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosStock_ProductosStock_ProductoStockId",
                table: "MovimientosStock");

            migrationBuilder.DropTable(
                name: "LoteDesignaciones");

            migrationBuilder.DropTable(
                name: "ProductosStock");

            migrationBuilder.DropTable(
                name: "FormatosEnvase");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosStock_ProductoStockId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "LoteId",
                table: "MovimientosStock");

            migrationBuilder.RenameColumn(
                name: "ProductoStockId",
                table: "MovimientosStock",
                newName: "ProductoId");

            migrationBuilder.AddColumn<int>(
                name: "Lote",
                table: "MovimientosStock",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
