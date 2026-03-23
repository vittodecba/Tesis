using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Graficos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LotesPrueba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecetaId = table.Column<int>(type: "int", nullable: false),
                    FermentadorId = table.Column<int>(type: "int", nullable: false),
                    PlanificacionProduccionId = table.Column<int>(type: "int", nullable: true),
                    FechaElaboracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Inoculo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiasEstimadosFermentacion = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaFinReal = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotesPrueba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotesPrueba_Fermentadores_FermentadorId",
                        column: x => x.FermentadorId,
                        principalTable: "Fermentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LotesPrueba_PlanificacionProduccion_PlanificacionProduccionId",
                        column: x => x.PlanificacionProduccionId,
                        principalTable: "PlanificacionProduccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LotesPrueba_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "IdReceta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosFermentacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoteId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaFermentacion = table.Column<int>(type: "int", nullable: false),
                    Ph = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    Densidad = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    Temperatura = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Presion = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Purgas = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Extracciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Agregados = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosFermentacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosFermentacion_LotesPrueba_LoteId",
                        column: x => x.LoteId,
                        principalTable: "LotesPrueba",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotesPrueba_FermentadorId",
                table: "LotesPrueba",
                column: "FermentadorId");

            migrationBuilder.CreateIndex(
                name: "IX_LotesPrueba_PlanificacionProduccionId",
                table: "LotesPrueba",
                column: "PlanificacionProduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_LotesPrueba_RecetaId",
                table: "LotesPrueba",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lote_DiaFermentacion",
                table: "RegistrosFermentacion",
                columns: new[] { "LoteId", "DiaFermentacion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lote_FechaRegistro",
                table: "RegistrosFermentacion",
                columns: new[] { "LoteId", "Fecha" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosFermentacion");

            migrationBuilder.DropTable(
                name: "LotesPrueba");
        }
    }
}