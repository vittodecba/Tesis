using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoteActualId",
                table: "Barriles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barriles_LoteActualId",
                table: "Barriles",
                column: "LoteActualId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barriles_Lotes_LoteActualId",
                table: "Barriles",
                column: "LoteActualId",
                principalTable: "Lotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barriles_Lotes_LoteActualId",
                table: "Barriles");

            migrationBuilder.DropIndex(
                name: "IX_Barriles_LoteActualId",
                table: "Barriles");

            migrationBuilder.DropColumn(
                name: "LoteActualId",
                table: "Barriles");
        }
    }
}
