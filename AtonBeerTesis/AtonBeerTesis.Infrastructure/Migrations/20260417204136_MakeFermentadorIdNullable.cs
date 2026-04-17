using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeFermentadorIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lotes_Fermentadores_FermentadorId",
                table: "Lotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId",
                table: "PlanificacionProduccion");

            migrationBuilder.DropIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion");

            migrationBuilder.AlterColumn<int>(
                name: "FermentadorId",
                table: "PlanificacionProduccion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FermentadorId",
                table: "Lotes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion",
                columns: new[] { "FermentadorId", "FechaInicio" },
                unique: true,
                filter: "[FermentadorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Fermentadores_FermentadorId",
                table: "Lotes",
                column: "FermentadorId",
                principalTable: "Fermentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId",
                table: "PlanificacionProduccion",
                column: "FermentadorId",
                principalTable: "Fermentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lotes_Fermentadores_FermentadorId",
                table: "Lotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId",
                table: "PlanificacionProduccion");

            migrationBuilder.DropIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion");

            migrationBuilder.AlterColumn<int>(
                name: "FermentadorId",
                table: "PlanificacionProduccion",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FermentadorId",
                table: "Lotes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion",
                columns: new[] { "FermentadorId", "FechaInicio" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Fermentadores_FermentadorId",
                table: "Lotes",
                column: "FermentadorId",
                principalTable: "Fermentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId",
                table: "PlanificacionProduccion",
                column: "FermentadorId",
                principalTable: "Fermentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
