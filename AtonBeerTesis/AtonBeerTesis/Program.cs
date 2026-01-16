using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AtonBeerTesis.Infrastructure.Data.AtonBeerDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ---------------------------------------------------------
            // Inyección de Dependencias (Repositorios y Servicios)
            // ---------------------------------------------------------

            // ROLES
            builder.Services.AddScoped<AtonBeerTesis.Domain.Interfaces.IRolRepository, AtonBeerTesis.Infrastructure.Repositories.RolRepository>();
            builder.Services.AddScoped<AtonBeerTesis.Application.Interfaces.IRolService, AtonBeerTesis.Application.Services.RolService>();

            // USUARIOS (NUEVO)
            // Nota: Acá le puse "Repositorios" en español porque así creamos la carpeta recién.
            builder.Services.AddScoped<AtonBeerTesis.Domain.Interfaces.IUsuarioRepository, AtonBeerTesis.Infrastructure.Repositorios.UsuarioRepository>();
            builder.Services.AddScoped<AtonBeerTesis.Application.Interfaces.IUsuarioService, AtonBeerTesis.Application.Services.UsuarioService>();


            // ---------------------------------------------------------

            // Permitir que Angular se conecte
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("NuevaPolitica", app =>
                {
                    app.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("NuevaPolitica");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}