using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HacerTipoOpcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insumos_TiposInsumo_TipoInsumoId",
                table: "Insumos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoInsumoId",
                table: "Insumos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Insumos_TiposInsumo_TipoInsumoId",
                table: "Insumos",
                column: "TipoInsumoId",
                principalTable: "TiposInsumo",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insumos_TiposInsumo_TipoInsumoId",
                table: "Insumos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoInsumoId",
                table: "Insumos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Insumos_TiposInsumo_TipoInsumoId",
                table: "Insumos",
                column: "TipoInsumoId",
                principalTable: "TiposInsumo",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
