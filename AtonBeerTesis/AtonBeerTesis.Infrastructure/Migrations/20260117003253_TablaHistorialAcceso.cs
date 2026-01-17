using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TablaHistorialAcceso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "historialAccesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EmailIntentado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaIntento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exitoso = table.Column<bool>(type: "bit", nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historialAccesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historialAccesos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_historialAccesos_UsuarioId",
                table: "historialAccesos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historialAccesos");
        }
    }
}
