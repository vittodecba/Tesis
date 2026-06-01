using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class FormatoEnvaseService : IFormatoEnvaseService
    {
        private readonly IRepository<FormatoEnvase> _formatoRepository;
        private readonly IRepository<ProductoStock> _productoStockRepository;
        private readonly IRecetaRepository _recetaRepository;
        private readonly IRepository<LoteDesignacion> _designacionRepository;

        public FormatoEnvaseService(
            IRepository<FormatoEnvase> formatoRepository,
            IRepository<ProductoStock> productoStockRepository,
            IRecetaRepository recetaRepository,
            IRepository<LoteDesignacion> designacionRepository)
        {
            _formatoRepository = formatoRepository;
            _productoStockRepository = productoStockRepository;
            _recetaRepository = recetaRepository;
            _designacionRepository = designacionRepository;
        }

        public async Task<IEnumerable<FormatoEnvaseDto>> ObtenerTodosAsync()
        {
            var formatos = await _formatoRepository.FindAllAsync();
            var productosStock = await _productoStockRepository.FindAllAsync();
            var recetas = (await _recetaRepository.GetAllAsync())
                          .ToDictionary(r => r.IdReceta, r => r.Nombre);

            return formatos.Select(f => new FormatoEnvaseDto
            {
                Id = f.Id,
                Nombre = f.Nombre,
                CapacidadLitros = f.CapacidadLitros,
                EsRetornable = f.EsRetornable,
                Productos = productosStock
                    .Where(p => p.FormatoEnvaseId == f.Id)
                    .GroupBy(p => p.Estilo)
                    .SelectMany(g =>
                        g.Any(p => p.RecetaId != null)
                            ? g.Where(p => p.RecetaId != null)
                            : g)
                    .Select(p => new ProductoStockDto
                    {
                        Id = p.Id,
                        Estilo = p.Estilo,
                        RecetaId = p.RecetaId,
                        RecetaNombre = p.RecetaId.HasValue && recetas.TryGetValue(p.RecetaId.Value, out var nombre)
                                       ? nombre : null,
                        StockActual = p.StockActual
                    })
                    .OrderBy(p => p.Estilo)
                    .ThenBy(p => p.RecetaNombre)
                    .ToList()
            });
        }

        public async Task<FormatoEnvaseDto> CrearFormatoAsync(CreateFormatoEnvaseDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new Exception("El nombre del formato es obligatorio");
            if (dto.CapacidadLitros <= 0)
                throw new Exception("La capacidad en litros debe ser mayor a 0");

            var existentes = await _formatoRepository.FindAllAsync();
            if (existentes.Any(f =>
                f.Nombre.Equals(dto.Nombre.Trim(), StringComparison.OrdinalIgnoreCase) &&
                f.CapacidadLitros == dto.CapacidadLitros))
                throw new Exception($"Ya existe un formato '{dto.Nombre.Trim()}' con capacidad {dto.CapacidadLitros}L.");

            var formato = new FormatoEnvase
            {
                Nombre          = dto.Nombre.Trim(),
                CapacidadLitros = dto.CapacidadLitros,
                EsRetornable    = dto.Nombre.Trim()
                                      .Contains("barril", StringComparison.OrdinalIgnoreCase)
            };
            await _formatoRepository.AddAsync(formato);

            // Crear entrada de stock por cada receta existente con estilo
            var recetas = await _recetaRepository.GetAllAsync();
            var recetasConEstilo = recetas
                .Where(r => !string.IsNullOrWhiteSpace(r.Estilo))
                .ToList();

            foreach (var receta in recetasConEstilo)
            {
                await _productoStockRepository.AddAsync(new ProductoStock
                {
                    FormatoEnvaseId = formato.Id,
                    Estilo = receta.Estilo!,
                    RecetaId = receta.IdReceta,
                    StockActual = 0
                });
            }

            var productos = recetasConEstilo.Select(r => new ProductoStockDto
            {
                Estilo = r.Estilo!,
                RecetaId = r.IdReceta,
                StockActual = 0
            }).ToList();

            return new FormatoEnvaseDto
            {
                Id = formato.Id,
                Nombre = formato.Nombre,
                CapacidadLitros = formato.CapacidadLitros,
                Productos = productos
            };
        }

        public async Task<bool> EliminarFormatoAsync(int id)
        {
            var formato = await _formatoRepository.FindOneAsync(id);
            if (formato == null) return false;

            // Validar ANTES de borrar cualquier cosa

            // 1. Verificar si hay stock activo en algún ProductoStock de este formato
            var productos = await _productoStockRepository.FindAllAsync();
            var productosDelFormato = productos.Where(p => p.FormatoEnvaseId == id).ToList();
            var tieneStock = productosDelFormato.Any(p => p.StockActual > 0);
            if (tieneStock)
                throw new InvalidOperationException(
                    "No se puede eliminar: este formato tiene stock activo. Primero llevá el stock a 0 antes de eliminar el formato.");

            // 2. Verificar si hay lotes con designaciones de volumen para este formato
            var tieneDesignaciones = await _designacionRepository.CountAsync(d => d.FormatoEnvaseId == id) > 0;
            if (tieneDesignaciones)
                throw new InvalidOperationException(
                    "No se puede eliminar: hay lotes que usan este formato en sus designaciones de volumen.");

            foreach (var p in productosDelFormato)
                _productoStockRepository.Remove(p.Id);

            _formatoRepository.Remove(id);
            return true;
        }

        public async Task AgregarEstiloATodosLosFormatosAsync(string estilo, int? recetaId = null)
        {
            var formatos = await _formatoRepository.FindAllAsync();
            var productosExistentes = await _productoStockRepository.FindAllAsync();

            foreach (var formato in formatos)
            {
                bool yaExiste = productosExistentes.Any(p =>
                    p.FormatoEnvaseId == formato.Id &&
                    p.Estilo.Equals(estilo, StringComparison.OrdinalIgnoreCase) &&
                    p.RecetaId == recetaId);

                if (!yaExiste)
                {
                    await _productoStockRepository.AddAsync(new ProductoStock
                    {
                        FormatoEnvaseId = formato.Id,
                        Estilo = estilo,
                        RecetaId = recetaId,
                        StockActual = 0
                    });
                }
            }
        }
    }
}
