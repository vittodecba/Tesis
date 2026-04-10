using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialFinal : Migration
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
                    RazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "Fermentadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fermentadores", x => x.Id);
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
                name: "Recetas",
                columns: table => new
                {
                    IdReceta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BatchSizeLitros = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.IdReceta);
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
                name: "Lotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoLote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecetaId = table.Column<int>(type: "int", nullable: false),
                    VolumenLitros = table.Column<int>(type: "int", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FermentadorId = table.Column<int>(type: "int", nullable: false),
                    FechaElaboracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Inoculo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiasEstimadosFermentacion = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaFinReal = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lotes_Fermentadores_FermentadorId",
                        column: x => x.FermentadorId,
                        principalTable: "Fermentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lotes_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "IdReceta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasosElaboracion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temperatura = table.Column<int>(type: "int", nullable: false),
                    Tiempo = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    RecetaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasosElaboracion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasosElaboracion_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "IdReceta",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreInsumo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoInsumoId = table.Column<int>(type: "int", nullable: false),
                    unidadMedidaId = table.Column<int>(type: "int", nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "PlanificacionProduccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FermentadorId = table.Column<int>(type: "int", nullable: false),
                    LoteId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinEstimada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    InsumosConfirmados = table.Column<bool>(type: "bit", nullable: false),
                    FermentadorId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanificacionProduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId",
                        column: x => x.FermentadorId,
                        principalTable: "Fermentadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanificacionProduccion_Fermentadores_FermentadorId1",
                        column: x => x.FermentadorId1,
                        principalTable: "Fermentadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlanificacionProduccion_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historialAccesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                });

            migrationBuilder.CreateTable(
                name: "RecetaInsumos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecetaId = table.Column<int>(type: "int", nullable: false),
                    InsumoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    unidadMedidaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecetaInsumos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecetaInsumos_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecetaInsumos_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "IdReceta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecetaInsumos_unidadMedida_unidadMedidaId",
                        column: x => x.unidadMedidaId,
                        principalTable: "unidadMedida",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "LotePrueba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecetaId = table.Column<int>(type: "int", nullable: true),
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
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoteId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosFermentacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosFermentacion_LotePrueba_LoteId",
                        column: x => x.LoteId,
                        principalTable: "LotePrueba",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosFermentacion_Lotes_LoteId1",
                        column: x => x.LoteId1,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_historialAccesos_UsuarioId",
                table: "historialAccesos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_TipoInsumoId",
                table: "Insumos",
                column: "TipoInsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_unidadMedidaId",
                table: "Insumos",
                column: "unidadMedidaId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_FermentadorId",
                table: "Lotes",
                column: "FermentadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_RecetaId",
                table: "Lotes",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_PasosElaboracion_RecetaId",
                table: "PasosElaboracion",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fermentador_Fecha",
                table: "PlanificacionProduccion",
                columns: new[] { "FermentadorId", "FechaInicio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanificacionProduccion_FermentadorId1",
                table: "PlanificacionProduccion",
                column: "FermentadorId1");

            migrationBuilder.CreateIndex(
                name: "IX_PlanificacionProduccion_LoteId",
                table: "PlanificacionProduccion",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaInsumos_InsumoId",
                table: "RecetaInsumos",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaInsumos_RecetaId",
                table: "RecetaInsumos",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaInsumos_unidadMedidaId",
                table: "RecetaInsumos",
                column: "unidadMedidaId");

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

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFermentacion_LoteId1",
                table: "RegistrosFermentacion",
                column: "LoteId1");

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
                name: "historialAccesos");

            migrationBuilder.DropTable(
                name: "MovimientosStock");

            migrationBuilder.DropTable(
                name: "PasosElaboracion");

            migrationBuilder.DropTable(
                name: "ProductosPrueba");

            migrationBuilder.DropTable(
                name: "RecetaInsumos");

            migrationBuilder.DropTable(
                name: "RegistrosFermentacion");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "LotePrueba");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "TiposInsumo");

            migrationBuilder.DropTable(
                name: "unidadMedida");

            migrationBuilder.DropTable(
                name: "PlanificacionProduccion");

            migrationBuilder.DropTable(
                name: "Lotes");

            migrationBuilder.DropTable(
                name: "Fermentadores");

            migrationBuilder.DropTable(
                name: "Recetas");
        }
    }
}
