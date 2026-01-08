using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipocliente = table.Column<int>(type: "int", nullable: false),
                    EstadoCliente = table.Column<int>(type: "int", nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Ubicacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ContactoNombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ContactoTelefono = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ContactoEmail = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    UltimaCompra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimoPedido = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalPedidos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
