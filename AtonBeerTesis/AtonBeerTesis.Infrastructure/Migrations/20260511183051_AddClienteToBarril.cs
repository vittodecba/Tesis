using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteToBarril : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Barriles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barriles_ClienteId",
                table: "Barriles",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barriles_Clientes_ClienteId",
                table: "Barriles",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barriles_Clientes_ClienteId",
                table: "Barriles");

            migrationBuilder.DropIndex(
                name: "IX_Barriles_ClienteId",
                table: "Barriles");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Barriles");
        }
    }
}
