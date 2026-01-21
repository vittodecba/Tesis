using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Infrastructure.Data;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Infrastructure.Repositories;
using AtonBeerTesis.Application.Services;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Infrastructure.Services;

namespace AtonBeerTesis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configuración de Base de Datos
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 2. Controladores y Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 3. Configuración de CORS (Permitir Angular)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Angular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // 4. Inyección de Dependencias (Servicios y Repositorios)
            
            // --- CLIENTES (De Main) ---
            builder.Services.AddScoped<IClienteService, ClienteService>();
            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

            // --- USUARIOS Y ROLES (De tu rama) ---
            builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IRolRepository, RolRepository>();
            builder.Services.AddScoped<IRolService, RolService>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            
            // --- AUTENTICACIÓN Y TOKEN ---
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            var app = builder.Build();

            // 5. Pipeline de la aplicación
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Usamos la política "Angular" que definimos arriba
            app.UseCors("Angular");

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}