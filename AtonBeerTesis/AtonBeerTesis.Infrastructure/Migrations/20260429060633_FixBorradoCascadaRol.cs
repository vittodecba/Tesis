using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBorradoCascadaRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_LotePrueba_LoteId",
                table: "RegistrosFermentacion");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_Lotes_LoteId1",
                table: "RegistrosFermentacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_roles_RolId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "LotePrueba");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosFermentacion_LoteId1",
                table: "RegistrosFermentacion");

            migrationBuilder.DropColumn(
                name: "LoteId1",
                table: "RegistrosFermentacion");

            migrationBuilder.AddColumn<int>(
                name: "RolId1",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Factor",
                table: "unidadMedida",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId1",
                table: "Usuarios",
                column: "RolId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFermentacion_Lotes_LoteId",
                table: "RegistrosFermentacion",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_roles_RolId",
                table: "Usuarios",
                column: "RolId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_roles_RolId1",
                table: "Usuarios",
                column: "RolId1",
                principalTable: "roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_Lotes_LoteId",
                table: "RegistrosFermentacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_roles_RolId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_roles_RolId1",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_RolId1",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RolId1",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Factor",
                table: "unidadMedida");

            migrationBuilder.AddColumn<int>(
                name: "LoteId1",
                table: "RegistrosFermentacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LotePrueba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FermentadorId = table.Column<int>(type: "int", nullable: false),
                    PlanificacionProduccionId = table.Column<int>(type: "int", nullable: true),
                    RecetaId = table.Column<int>(type: "int", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiasEstimadosFermentacion = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaElaboracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Inoculo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotePrueba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotePrueba_Fermentadores_FermentadorId",
                        column: x => x.FermentadorId,
                        principalTable: "Fermentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LotePrueba_PlanificacionProduccion_PlanificacionProduccionId",
                        column: x => x.PlanificacionProduccionId,
                        principalTable: "PlanificacionProduccion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LotePrueba_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "IdReceta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFermentacion_LoteId1",
                table: "RegistrosFermentacion",
                column: "LoteId1");

            migrationBuilder.CreateIndex(
                name: "IX_LotePrueba_FermentadorId",
                table: "LotePrueba",
                column: "FermentadorId");

            migrationBuilder.CreateIndex(
                name: "IX_LotePrueba_PlanificacionProduccionId",
                table: "LotePrueba",
                column: "PlanificacionProduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_LotePrueba_RecetaId",
                table: "LotePrueba",
                column: "RecetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFermentacion_LotePrueba_LoteId",
                table: "RegistrosFermentacion",
                column: "LoteId",
                principalTable: "LotePrueba",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFermentacion_Lotes_LoteId1",
                table: "RegistrosFermentacion",
                column: "LoteId1",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_roles_RolId",
                table: "Usuarios",
                column: "RolId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
