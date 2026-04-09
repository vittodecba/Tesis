using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecetaIdNullableEnLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LotesPrueba_Fermentadores_FermentadorId",
                table: "LotesPrueba");

            migrationBuilder.DropForeignKey(
                name: "FK_LotesPrueba_PlanificacionProduccion_PlanificacionProduccionId",
                table: "LotesPrueba");

            migrationBuilder.DropForeignKey(
                name: "FK_LotesPrueba_Recetas_RecetaId",
                table: "LotesPrueba");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_LotesPrueba_LoteId",
                table: "RegistrosFermentacion");

            migrationBuilder.DropTable(
                name: "FermentadoresPruebas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LotesPrueba",
                table: "LotesPrueba");

            migrationBuilder.RenameTable(
                name: "LotesPrueba",
                newName: "LotePrueba");

            migrationBuilder.RenameIndex(
                name: "IX_LotesPrueba_RecetaId",
                table: "LotePrueba",
                newName: "IX_LotePrueba_RecetaId");

            migrationBuilder.RenameIndex(
                name: "IX_LotesPrueba_PlanificacionProduccionId",
                table: "LotePrueba",
                newName: "IX_LotePrueba_PlanificacionProduccionId");

            migrationBuilder.RenameIndex(
                name: "IX_LotesPrueba_FermentadorId",
                table: "LotePrueba",
                newName: "IX_LotePrueba_FermentadorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LotePrueba",
                table: "LotePrueba",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LotePrueba_Fermentadores_FermentadorId",
                table: "LotePrueba",
                column: "FermentadorId",
                principalTable: "Fermentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LotePrueba_PlanificacionProduccion_PlanificacionProduccionId",
                table: "LotePrueba",
                column: "PlanificacionProduccionId",
                principalTable: "PlanificacionProduccion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LotePrueba_Recetas_RecetaId",
                table: "LotePrueba",
                column: "RecetaId",
                principalTable: "Recetas",
                principalColumn: "IdReceta");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFermentacion_LotePrueba_LoteId",
                table: "RegistrosFermentacion",
                column: "LoteId",
                principalTable: "LotePrueba",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LotePrueba_Fermentadores_FermentadorId",
                table: "LotePrueba");

            migrationBuilder.DropForeignKey(
                name: "FK_LotePrueba_PlanificacionProduccion_PlanificacionProduccionId",
                table: "LotePrueba");

            migrationBuilder.DropForeignKey(
                name: "FK_LotePrueba_Recetas_RecetaId",
                table: "LotePrueba");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_LotePrueba_LoteId",
                table: "RegistrosFermentacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LotePrueba",
                table: "LotePrueba");

            migrationBuilder.RenameTable(
                name: "LotePrueba",
                newName: "LotesPrueba");

            migrationBuilder.RenameIndex(
                name: "IX_LotePrueba_RecetaId",
                table: "LotesPrueba",
                newName: "IX_LotesPrueba_RecetaId");

            migrationBuilder.RenameIndex(
                name: "IX_LotePrueba_PlanificacionProduccionId",
                table: "LotesPrueba",
                newName: "IX_LotesPrueba_PlanificacionProduccionId");

            migrationBuilder.RenameIndex(
                name: "IX_LotePrueba_FermentadorId",
                table: "LotesPrueba",
                newName: "IX_LotesPrueba_FermentadorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LotesPrueba",
                table: "LotesPrueba",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FermentadoresPruebas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Disponibilidad = table.Column<bool>(type: "bit", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FermentadoresPruebas", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_LotesPrueba_Fermentadores_FermentadorId",
                table: "LotesPrueba",
                column: "FermentadorId",
                principalTable: "Fermentadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LotesPrueba_PlanificacionProduccion_PlanificacionProduccionId",
                table: "LotesPrueba",
                column: "PlanificacionProduccionId",
                principalTable: "PlanificacionProduccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LotesPrueba_Recetas_RecetaId",
                table: "LotesPrueba",
                column: "RecetaId",
                principalTable: "Recetas",
                principalColumn: "IdReceta",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFermentacion_LotesPrueba_LoteId",
                table: "RegistrosFermentacion",
                column: "LoteId",
                principalTable: "LotesPrueba",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
