using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dtos.Recetas;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Application.Services
{
    public class RecetaService : IRecetaService
    {
        private readonly IRecetaRepository _recetaRepository;

        public RecetaService(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<List<RecetaDto>> GetAllAsync(string? nombre = null, string? estilo = null, string? estado = null, string? orden = null)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                estado = "Activa";
            }

            var recetas = await _recetaRepository.GetAllAsync(nombre, estilo, estado, orden);
            return recetas.Select(MapToDto).ToList();
        }

        public async Task<RecetaDto?> GetByIdAsync(int id)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            return receta is null ? null : MapToDto(receta);
        }

        public async Task<int> CreateAsync(CreateRecetaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new Exception("El nombre de la receta es obligatorio");

            var nombre = dto.Nombre.Trim();

            if (await ExisteNombreAsync(nombre))
                throw new Exception("Ya existe una receta con ese nombre");

            if (dto.BatchSizeLitros <= 0)
                throw new Exception("El volumen (BatchSizeLitros) debe ser mayor a 0");

            var receta = new Receta
            {
                Nombre = nombre,
                Estilo = dto.Estilo?.Trim() ?? "",
                BatchSizeLitros = dto.BatchSizeLitros,
                Notas = dto.Notas?.Trim(),
                Estado = EstadoReceta.Activa,
                FechaCreacion = DateTime.UtcNow,
                RecetaInsumos = dto.RecetaInsumos.Select(i => new RecetaInsumo
                {
                    InsumoId = i.InsumoId,
                    Cantidad = i.Cantidad,
                    unidadMedidaId = i.UnidadMedidaId
                }).ToList()
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

            if (!Enum.TryParse<EstadoReceta>(dto.Estado, true, out var estadoEnum))
                throw new Exception("Estado de receta inválido");
            //Datos que se actualizan
            receta.Nombre = nombre;
            receta.Estilo = dto.Estilo?.Trim() ?? "";
            receta.BatchSizeLitros = dto.BatchSizeLitros;
            receta.Notas = dto.Notas?.Trim();
            receta.Estado = estadoEnum;
            receta.FechaActualizacion = DateTime.UtcNow;
            //ACTUALIZACIÓN DE INSUMOS
            // Borramos los que tiene actualmente la receta en memoria
            receta.RecetaInsumos.Clear();
            // Agrego los que vienen del DTO
            if (dto.RecetaInsumos != null)
            {
                foreach (var i in dto.RecetaInsumos)
                {
                    receta.RecetaInsumos.Add(new RecetaInsumo
                    {
                        InsumoId = i.InsumoId,
                        Cantidad = i.Cantidad,
                        unidadMedidaId = i.UnidadMedidaId,
                    });
                }
            }

            await _recetaRepository.UpdateAsync(receta);
            return true;
        }

        public async Task<bool> PatchAsync(int id, PatchRecetaDto dto)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta is null) return false;

            if (dto.Nombre is not null)
            {
                var nombre = dto.Nombre.Trim();
                if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("El nombre no puede quedar vacío");
                if (await ExisteNombreAsync(nombre, id)) throw new Exception("Ya existe otra receta con ese nombre");
                receta.Nombre = nombre;
            }

            if (dto.Estilo is not null) receta.Estilo = dto.Estilo.Trim();
            if (dto.Notas is not null) receta.Notas = dto.Notas.Trim();
            if (dto.BatchSizeLitros.HasValue)
            {
                if (dto.BatchSizeLitros.Value <= 0) throw new Exception("El volumen debe ser mayor a 0");
                receta.BatchSizeLitros = dto.BatchSizeLitros.Value;
            }

            if (dto.Estado is not null)
            {
                if (!Enum.TryParse<EstadoReceta>(dto.Estado, true, out var estadoEnum)) throw new Exception("Estado inválido");
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

        public List<string> GetEstadosReceta() => Enum.GetNames(typeof(EstadoReceta)).ToList();              
        private RecetaDto MapToDto(Receta receta)
        {
            return new RecetaDto
            {
                IdReceta = receta.IdReceta,
                Nombre = receta.Nombre,
                Estilo = receta.Estilo,
                BatchSizeLitros = receta.BatchSizeLitros,
                Notas = receta.Notas,
                Estado = receta.Estado.ToString(),
                FechaCreacion = receta.FechaCreacion,
                FechaActualizacion = receta.FechaActualizacion,
                
                RecetaInsumos = receta.RecetaInsumos?.Select(ri => new RecetaInsumoDto
                {
                    InsumoId = ri.InsumoId,
                    Cantidad = ri.Cantidad,
                    NombreInsumo = ri.Insumo?.NombreInsumo,
                    UnidadMedidaId = ri.unidadMedidaId,
                    UnidadMedida = ri.unidadMedida?.Abreviatura ?? ""
                }).ToList() ?? new List<RecetaInsumoDto>(),

                PasosElaboracion = receta.PasosElaboracion?.Select(p => new PasosElaboracionDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Temperatura = p.Temperatura,
                    Tiempo = p.Tiempo,
                    Orden = p.Orden
                }).ToList() ?? new List<PasosElaboracionDto>()
            };
        }

        private async Task<bool> ExisteNombreAsync(string nombre, int? idExcluido = null)
        {
            var todas = await _recetaRepository.GetAllAsync();
            var coincidencia = todas.Where(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

            if (idExcluido.HasValue)
                return coincidencia.Any(r => r.IdReceta != idExcluido.Value);

            return coincidencia.Any();
        }

        public async Task<bool> AddInsumoToReceta(int id, RecetaInsumoDto dto)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta == null) return false;

            var existente = receta.RecetaInsumos
                .FirstOrDefault(x => x.InsumoId == dto.InsumoId);

            if (existente != null)
            {
                existente.Cantidad = dto.Cantidad;
                existente.unidadMedidaId = dto.UnidadMedidaId;

                await _recetaRepository.UpdateAsync(receta);
                return true;
            }
            else
            {
                var nuevaRelacion = new RecetaInsumo
                {
                    RecetaId = id,
                    InsumoId = dto.InsumoId,
                    Cantidad = dto.Cantidad,
                    unidadMedidaId = dto.UnidadMedidaId
                };

                return await _recetaRepository.AddInsumoAsync(nuevaRelacion);
            }
        }

        public async Task<bool> RemoveInsumoDeReceta(int id, int insumoId)
        {
            return await _recetaRepository.RemoveInsumoAsync(id, insumoId);
        }

        public async Task<PasosElaboracion> CrearPasoAsync(int recetaId, PasosElaboracion paso)
        {
            var recetaExistente = await _recetaRepository.GetByIdAsync(recetaId);
            if (recetaExistente == null) throw new Exception("La receta no existe.");

            paso.RecetaId = recetaId;
            return await _recetaRepository.AddPasoAsync(paso);
        }

        public async Task<bool> EditarPasoAsync(int recetaId, int pasoId, PasosElaboracion pasoEditado)
        {
            var receta = await _recetaRepository.GetByIdAsync(recetaId);
            if (receta == null || receta.PasosElaboracion == null) return false;

            var pasoExistente = receta.PasosElaboracion.FirstOrDefault(p => p.Id == pasoId);
            if (pasoExistente == null) return false;

            pasoExistente.Nombre = pasoEditado.Nombre;
            pasoExistente.Descripcion = pasoEditado.Descripcion;
            pasoExistente.Temperatura = pasoEditado.Temperatura;
            pasoExistente.Tiempo = pasoEditado.Tiempo;
            pasoExistente.Orden = pasoEditado.Orden;

            return await _recetaRepository.UpdatePasoAsync(pasoExistente);
        }

        public async Task<bool> EliminarPasoAsync(int recetaId, int pasoId)
        {
            return await _recetaRepository.DeletePasoAsync(pasoId);
        }
    }
}