using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Services;


using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Add services to the container.

            builder.Services.AddDbContext<AtonBeerTesis.Infrastructure.Data.AtonBeerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // Inyección de Dependencias
            builder.Services.AddScoped<AtonBeerTesis.Domain.Interfaces.IRolRepository, AtonBeerTesis.Infrastructure.Repositories.RolRepository>();
            builder.Services.AddScoped<AtonBeerTesis.Application.Interfaces.IRolService, AtonBeerTesis.Application.Services.RolService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
