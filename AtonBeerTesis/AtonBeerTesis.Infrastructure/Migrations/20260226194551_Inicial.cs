using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "RecetaInsumos",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "unidadMedidaId",
                table: "RecetaInsumos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RecetaInsumos_unidadMedidaId",
                table: "RecetaInsumos",
                column: "unidadMedidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecetaInsumos_unidadMedida_unidadMedidaId",
                table: "RecetaInsumos",
                column: "unidadMedidaId",
                principalTable: "unidadMedida",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecetaInsumos_unidadMedida_unidadMedidaId",
                table: "RecetaInsumos");

            migrationBuilder.DropIndex(
                name: "IX_RecetaInsumos_unidadMedidaId",
                table: "RecetaInsumos");

            migrationBuilder.DropColumn(
                name: "unidadMedidaId",
                table: "RecetaInsumos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "RecetaInsumos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);
        }
    }
}
