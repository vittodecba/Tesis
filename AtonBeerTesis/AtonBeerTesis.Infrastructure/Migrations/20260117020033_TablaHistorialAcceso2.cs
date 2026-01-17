using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TablaHistorialAcceso2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_historialAccesos_Usuarios_UsuarioId",
                table: "historialAccesos");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "historialAccesos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_historialAccesos_Usuarios_UsuarioId",
                table: "historialAccesos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_historialAccesos_Usuarios_UsuarioId",
                table: "historialAccesos");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "historialAccesos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_historialAccesos_Usuarios_UsuarioId",
                table: "historialAccesos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
