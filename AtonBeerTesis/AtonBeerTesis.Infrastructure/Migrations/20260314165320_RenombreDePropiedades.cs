using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenombreDePropiedades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaFin",
                table: "PlanificacionProduccion",
                newName: "FechaFinEstimada");

            migrationBuilder.AlterColumn<int>(
                name: "VolumenLitros",
                table: "Lotes",
                type: "int",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaFinEstimada",
                table: "PlanificacionProduccion",
                newName: "FechaFin");

            migrationBuilder.AlterColumn<decimal>(
                name: "VolumenLitros",
                table: "Lotes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
