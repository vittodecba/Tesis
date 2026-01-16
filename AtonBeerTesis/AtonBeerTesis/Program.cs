using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Infrastructure.Data;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entidades;
using AtonBeerTesis.Infrastructure.Repositories;
using AtonBeerTesis.Application.Services;
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
            //Agrego el servicio de Token
            builder.Services.AddScoped<ITokenService, TokenService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
