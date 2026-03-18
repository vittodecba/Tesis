using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReestructuracionPlanProduccionYLotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanificacionProduccion_Recetas_RecetaId",
                table: "PlanificacionProduccion");

            migrationBuilder.RenameColumn(
                name: "RecetaId",
                table: "PlanificacionProduccion",
                newName: "LoteId");

            migrationBuilder.RenameColumn(
                name: "FechaProduccion",
                table: "PlanificacionProduccion",
                newName: "FechaInicio");

            migrationBuilder.RenameIndex(
                name: "IX_PlanificacionProduccion_RecetaId",
                table: "PlanificacionProduccion",
                newName: "IX_PlanificacionProduccion_LoteId");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "PlanificacionProduccion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Lotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoLote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecetaId = table.Column<int>(type: "int", nullable: false),
                    VolumenLitros = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lotes_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "IdReceta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_RecetaId",
                table: "Lotes",
                column: "RecetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanificacionProduccion_Lotes_LoteId",
                table: "PlanificacionProduccion",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanificacionProduccion_Lotes_LoteId",
                table: "PlanificacionProduccion");

            migrationBuilder.DropTable(
                name: "Lotes");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "PlanificacionProduccion");

            migrationBuilder.RenameColumn(
                name: "LoteId",
                table: "PlanificacionProduccion",
                newName: "RecetaId");

            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "PlanificacionProduccion",
                newName: "FechaProduccion");

            migrationBuilder.RenameIndex(
                name: "IX_PlanificacionProduccion_LoteId",
                table: "PlanificacionProduccion",
                newName: "IX_PlanificacionProduccion_RecetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanificacionProduccion_Recetas_RecetaId",
                table: "PlanificacionProduccion",
                column: "RecetaId",
                principalTable: "Recetas",
                principalColumn: "IdReceta",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
