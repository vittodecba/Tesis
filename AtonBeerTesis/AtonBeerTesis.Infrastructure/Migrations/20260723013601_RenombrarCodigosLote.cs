using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarCodigosLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombra todos los lotes existentes al nuevo esquema corto "L-{Id}".
            // (No cambia el modelo; es solo migración de datos.)
            migrationBuilder.Sql("UPDATE Lotes SET CodigoLote = CONCAT('L-', CAST(Id AS varchar(20)));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // El formato viejo (L-yyyyMMdd-NNN / LOTE-aaaa-NNN) no es reconstruible; no-op.
        }
    }
}
