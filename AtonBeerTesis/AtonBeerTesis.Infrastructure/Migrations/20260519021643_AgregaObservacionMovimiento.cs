using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregaObservacionMovimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "MovimientosBarril",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "MovimientosBarril");
        }
    }
}
