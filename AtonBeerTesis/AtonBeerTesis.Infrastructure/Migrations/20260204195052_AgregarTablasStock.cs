using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260211154553_Insumos.cs
    public partial class Insumos : Migration
========
    public partial class AgregarTablasStock : Migration
>>>>>>>> Feature/Gestion-Stock:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260204195052_AgregarTablasStock.cs
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
                    Cuit = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ubicacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ContactoNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactoTelefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactoEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UltimaCompra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimoPedido = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalPedidos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Lote = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoMovimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockPrevio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StockResultante = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductosPrueba",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Formato = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosPrueba", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposInsumo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposInsumo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "unidadMedida",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abreviatura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unidadMedida", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    TokenRecuperacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenExpiracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_roles_RolId",
                        column: x => x.RolId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
<<<<<<<< HEAD:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260211154553_Insumos.cs
                name: "Insumos",
========
                name: "historialAccesos",
>>>>>>>> Feature/Gestion-Stock:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260204195052_AgregarTablasStock.cs
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
<<<<<<<< HEAD:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260211154553_Insumos.cs
                    NombreInsumo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoInsumoId = table.Column<int>(type: "int", nullable: false),
                    unidadMedidaId = table.Column<int>(type: "int", nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Insumos_TiposInsumo_TipoInsumoId",
                        column: x => x.TipoInsumoId,
                        principalTable: "TiposInsumo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Insumos_unidadMedida_unidadMedidaId",
                        column: x => x.unidadMedidaId,
                        principalTable: "unidadMedida",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
========
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
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
                        principalColumn: "Id");
>>>>>>>> Feature/Gestion-Stock:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260204195052_AgregarTablasStock.cs
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "...", "Cocinero" },
                    { 2, "...", "ResponsablePlanta" },
                    { 3, "...", "ResponsablePedidos" },
                    { 4, "...", "Gerente" },
                    { 5, "...", "GerenteMayor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cuit",
                table: "Clientes",
                column: "Cuit",
                unique: true);

            migrationBuilder.CreateIndex(
<<<<<<<< HEAD:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260211154553_Insumos.cs
                name: "IX_Insumos_TipoInsumoId",
                table: "Insumos",
                column: "TipoInsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_unidadMedidaId",
                table: "Insumos",
                column: "unidadMedidaId");
========
                name: "IX_historialAccesos_UsuarioId",
                table: "historialAccesos",
                column: "UsuarioId");
>>>>>>>> Feature/Gestion-Stock:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260204195052_AgregarTablasStock.cs

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
<<<<<<<< HEAD:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260211154553_Insumos.cs
                name: "Insumos");
========
                name: "historialAccesos");

            migrationBuilder.DropTable(
                name: "MovimientosStock");

            migrationBuilder.DropTable(
                name: "ProductosPrueba");
>>>>>>>> Feature/Gestion-Stock:AtonBeerTesis/AtonBeerTesis.Infrastructure/Migrations/20260204195052_AgregarTablasStock.cs

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "TiposInsumo");

            migrationBuilder.DropTable(
                name: "unidadMedida");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
