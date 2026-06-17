using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtonBeerTesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoAtrasado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [EstadosPedido] WHERE [Id] = 5)
BEGIN
    SET IDENTITY_INSERT [EstadosPedido] ON;

    INSERT INTO [EstadosPedido] ([Id], [Nombre])
    VALUES (5, N'Atrasado');

    SET IDENTITY_INSERT [EstadosPedido] OFF;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [EstadosPedido] WHERE [Id] = 5)
BEGIN
    DELETE FROM [EstadosPedido]
    WHERE [Id] = 5;
END
");
        }
    }
}