using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Infrastructure.Data;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entidades;
using AtonBeerTesis.Infrastructure.Repositories;
namespace AtonBeerTesis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            //Aca leemos la cadena de conexion desde el appsettings.json y configuramos el DbContext para que use SQL Server
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            //1Agrego el repo generico para que pueda ser inyectado en los controladores
            builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            //2 Agrego el repo especifico de Usuario
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Configuracion de CORS para permitir solicitudes desde el front-end Angular
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy => policy.WithOrigins("http://localhost:4200") // La URL del Front Angular
                                    .AllowAnyMethod()                     // Permite GET, POST, PUT, DELETE
                                    .AllowAnyHeader());                   // Permite headers de JSON
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngular"); // Habilita la politica de CORS
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
