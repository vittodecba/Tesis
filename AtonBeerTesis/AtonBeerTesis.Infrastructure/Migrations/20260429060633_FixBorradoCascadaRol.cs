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
            // Eliminar el FK actual de RegistrosFermentacion → Lotes para recrearlo correctamente
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFermentacion_Lotes_LoteId",
                table: "RegistrosFermentacion");

            // Eliminar el FK de Usuarios → roles para recrearlo con Restrict
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Usuarios_roles_RolId')
                    ALTER TABLE [Usuarios] DROP CONSTRAINT [FK_Usuarios_roles_RolId];
            ");

            // Eliminar índice LoteId1 si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RegistrosFermentacion_LoteId1'
                           AND object_id = OBJECT_ID('RegistrosFermentacion'))
                    DROP INDEX [IX_RegistrosFermentacion_LoteId1] ON [RegistrosFermentacion];
            ");

            // Eliminar columna LoteId1 si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                           WHERE TABLE_NAME = 'RegistrosFermentacion' AND COLUMN_NAME = 'LoteId1')
                    ALTER TABLE [RegistrosFermentacion] DROP COLUMN [LoteId1];
            ");

            // Eliminar tabla LotePrueba si existe (quitando sus FKs primero)
            migrationBuilder.Sql(@"
                IF OBJECT_ID('LotePrueba') IS NOT NULL BEGIN
                    DECLARE @sql NVARCHAR(MAX) = '';
                    SELECT @sql += 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; '
                    FROM sys.foreign_keys fk
                    JOIN sys.tables t ON fk.parent_object_id = t.object_id
                    WHERE fk.referenced_object_id = OBJECT_ID('LotePrueba');
                    IF @sql <> '' EXEC sp_executesql @sql;
                    DROP TABLE [LotePrueba];
                END
            ");

            // Agregar RolId1 a Usuarios si no existe
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                               WHERE TABLE_NAME = 'Usuarios' AND COLUMN_NAME = 'RolId1')
                    ALTER TABLE [Usuarios] ADD [RolId1] int NULL;
            ");

            // Agregar Factor a unidadMedida si no existe
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                               WHERE TABLE_NAME = 'unidadMedida' AND COLUMN_NAME = 'Factor')
                    ALTER TABLE [unidadMedida] ADD [Factor] float NOT NULL DEFAULT 0.0;
            ");

            // Crear índice IX_Usuarios_RolId1 si no existe
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Usuarios_RolId1'
                               AND object_id = OBJECT_ID('Usuarios'))
                    CREATE INDEX [IX_Usuarios_RolId1] ON [Usuarios] ([RolId1]);
            ");

            // Recrear FK de RegistrosFermentacion → Lotes con Cascade
            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFermentacion_Lotes_LoteId",
                table: "RegistrosFermentacion",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Recrear FK de Usuarios → roles con Restrict
            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_roles_RolId",
                table: "Usuarios",
                column: "RolId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Agregar FK de RolId1
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Usuarios_roles_RolId1')
                    ALTER TABLE [Usuarios] ADD CONSTRAINT [FK_Usuarios_roles_RolId1]
                    FOREIGN KEY ([RolId1]) REFERENCES [roles] ([Id]);
            ");
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
