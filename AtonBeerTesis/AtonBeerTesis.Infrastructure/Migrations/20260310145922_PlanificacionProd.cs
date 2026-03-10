using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PlanificacionProd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FermentadoresPruebas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disponibilidad = table.Column<bool>(type: "bit", nullable: false),
                    Capacidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FermentadoresPruebas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanificacionProduccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecetaId = table.Column<int>(type: "int", nullable: false),
                    FermentadorId = table.Column<int>(type: "int", nullable: false),
                    FechaProduccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InsumosConfirmados = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanificacionProduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanificacionProduccion_FermentadoresPruebas_FermentadorId",
                        column: x => x.FermentadorId,
                        principalTable: "FermentadoresPruebas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanificacionProduccion_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "IdReceta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion",
                columns: new[] { "FermentadorId", "FechaProduccion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanificacionProduccion_RecetaId",
                table: "PlanificacionProduccion",
                column: "RecetaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanificacionProduccion");

            migrationBuilder.DropTable(
                name: "FermentadoresPruebas");
        }
    }
}
