using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedEstadosPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM EstadosPedido WHERE Id = 1)
                    INSERT INTO EstadosPedido (Id, Nombre) VALUES (1, 'Pendiente');
                IF NOT EXISTS (SELECT 1 FROM EstadosPedido WHERE Id = 2)
                    INSERT INTO EstadosPedido (Id, Nombre) VALUES (2, 'Entregado');
                IF NOT EXISTS (SELECT 1 FROM EstadosPedido WHERE Id = 3)
                    INSERT INTO EstadosPedido (Id, Nombre) VALUES (3, 'Facturado');
                IF NOT EXISTS (SELECT 1 FROM EstadosPedido WHERE Id = 4)
                    INSERT INTO EstadosPedido (Id, Nombre) VALUES (4, 'Cancelado');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
