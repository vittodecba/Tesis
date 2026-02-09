using AtonBeerTesis.Application.Dtos.Recetas;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public class RecetaService : IRecetaService
    {
        private readonly IRecetaRepository _recetaRepository;

        public RecetaService(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<List<RecetaDto>> GetAllAsync(string? estilo = null, string? estado = null)
        {
            var recetas = await _recetaRepository.GetAllAsync();

            // Estado: por defecto solo Activas (igual que hiciste con Cliente Activo)
            if (!string.IsNullOrWhiteSpace(estado) &&
                Enum.TryParse<EstadoReceta>(estado, true, out var estadoEnum))
            {
                recetas = recetas.Where(r => r.Estado == estadoEnum).ToList();
            }
            else
            {
                recetas = recetas.Where(r => r.Estado == EstadoReceta.Activa).ToList();
            }

            // Estilo (filtro por texto)
            if (!string.IsNullOrWhiteSpace(estilo))
            {
                recetas = recetas
                    .Where(r => r.Estilo != null && r.Estilo.Contains(estilo, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return recetas.Select(MapToDto).ToList();
        }

        public async Task<RecetaDto?> GetByIdAsync(int id)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            return receta is null ? null : MapToDto(receta);
        }

        public async Task<int> CreateAsync(CrearRecetaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new Exception("El nombre de la receta es obligatorio");

            var nombre = dto.Nombre.Trim();

            if (await ExisteNombreAsync(nombre))
                throw new Exception("Ya existe una receta con ese nombre");

            // Estilo puede ser null
            var estilo = dto.Estilo?.Trim();

            if (dto.BatchSizeLitros <= 0)
                throw new Exception("El volumen (BatchSizeLitros) debe ser mayor a 0");

            var receta = new Receta
            {
                Nombre = nombre,
                Estilo = estilo ?? "",
                BatchSizeLitros = dto.BatchSizeLitros,
                Notas = dto.Notas?.Trim(),
                Estado = EstadoReceta.Activa, // o Borrador si preferís
                FechaCreacion = DateTime.UtcNow
            };

            await _recetaRepository.AddAsync(receta);
            return receta.IdReceta;
        }

        public async Task<bool> UpdateAsync(int id, ActualizarRecetaDto dto)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta is null) return false;

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new Exception("El nombre de la receta es obligatorio");

            var nombre = dto.Nombre.Trim();

            if (await ExisteNombreAsync(nombre, id))
                throw new Exception("Ya existe otra receta con ese nombre");

            if (dto.BatchSizeLitros <= 0)
                throw new Exception("El volumen (BatchSizeLitros) debe ser mayor a 0");

            // Estado
            if (!Enum.TryParse<EstadoReceta>(dto.Estado, true, out var estadoEnum))
                throw new Exception("Estado de receta inválido");

            receta.Nombre = nombre;
            receta.Estilo = dto.Estilo?.Trim() ?? "";
            receta.BatchSizeLitros = dto.BatchSizeLitros;
            receta.Notas = dto.Notas?.Trim();
            receta.Estado = estadoEnum;
            receta.FechaActualizacion = DateTime.UtcNow;

            await _recetaRepository.UpdateAsync(receta);
            return true;
        }

        public async Task<bool> PatchAsync(int id, PatchRecetaDto dto)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta is null) return false;

            // Nombre
            if (dto.Nombre is not null)
            {
                var nombre = dto.Nombre.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                    throw new Exception("El nombre no puede quedar vacío");

                if (await ExisteNombreAsync(nombre, id))
                    throw new Exception("Ya existe otra receta con ese nombre");

                receta.Nombre = nombre;
            }

            // Strings simples
            if (dto.Estilo is not null) receta.Estilo = dto.Estilo.Trim();
            if (dto.Notas is not null) receta.Notas = dto.Notas.Trim();

            // Numeros
            if (dto.BatchSizeLitros.HasValue)
            {
                if (dto.BatchSizeLitros.Value <= 0)
                    throw new Exception("El volumen (BatchSizeLitros) debe ser mayor a 0");

                receta.BatchSizeLitros = dto.BatchSizeLitros.Value;
            }

            // Enum Estado
            if (dto.Estado is not null)
            {
                if (!Enum.TryParse<EstadoReceta>(dto.Estado, true, out var estadoEnum))
                    throw new Exception("Estado de receta inválido");

                receta.Estado = estadoEnum;
            }

            receta.FechaActualizacion = DateTime.UtcNow;

            await _recetaRepository.UpdateAsync(receta);
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta is null) return false;

            receta.Estado = EstadoReceta.Inactiva;
            receta.FechaActualizacion = DateTime.UtcNow;

            await _recetaRepository.UpdateAsync(receta);
            return true;
        }

        public List<string> GetEstadosReceta() => Enum.GetNames(typeof(EstadoRece
