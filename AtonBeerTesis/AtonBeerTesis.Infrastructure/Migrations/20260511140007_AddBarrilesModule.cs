using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBarrilesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsRetornable",
                table: "FormatosEnvase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Barriles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormatoEnvaseId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoteId = table.Column<int>(type: "int", nullable: true),
                    FechaAdquisicion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barriles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Barriles_FormatosEnvase_FormatoEnvaseId",
                        column: x => x.FormatoEnvaseId,
                        principalTable: "FormatosEnvase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosBarril",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarrilId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoAnterior = table.Column<int>(type: "int", nullable: false),
                    EstadoNuevo = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosBarril", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosBarril_Barriles_BarrilId",
                        column: x => x.BarrilId,
                        principalTable: "Barriles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Barriles_Codigo",
                table: "Barriles",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barriles_FormatoEnvaseId",
                table: "Barriles",
                column: "FormatoEnvaseId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBarril_BarrilId",
                table: "MovimientosBarril",
                column: "BarrilId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosBarril");

            migrationBuilder.DropTable(
                name: "Barriles");

            migrationBuilder.DropColumn(
                name: "EsRetornable",
                table: "FormatosEnvase");
        }
    }
}
