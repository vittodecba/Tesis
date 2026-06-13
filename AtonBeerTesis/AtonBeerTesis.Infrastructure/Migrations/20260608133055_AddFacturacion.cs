using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacturacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CondicionIVA",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = ResponsableInscripto (default para clientes existentes)

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DomicilioComercial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CondicionIVA = table.Column<int>(type: "int", nullable: false),
                    PuntoVenta = table.Column<int>(type: "int", nullable: false),
                    IngresosBrutos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InicioActividades = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    PuntoVenta = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NetoGravado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Iva = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RutaPdf = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facturas_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "CondicionIVA", "Cuit", "DomicilioComercial", "IngresosBrutos", "InicioActividades", "PuntoVenta", "RazonSocial" },
                values: new object[] { 1, 1, "30-00000000-0", "Domicilio comercial a completar", "", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "AtonBeer S.A." });

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_VentaId",
                table: "Facturas",
                column: "VentaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropColumn(
                name: "CondicionIVA",
                table: "Clientes");
        }
    }
}
