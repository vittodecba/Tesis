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

            return formatos.Select(f => new FormatoEnvaseDto
            {
                Id = f.Id,
                Nombre = f.Nombre,
                CapacidadLitros = f.CapacidadLitros,
                Productos = productosStock
                    .Where(p => p.FormatoEnvaseId == f.Id)
                    .Select(p => new ProductoStockDto
                    {
                        Id = p.Id,
                        Estilo = p.Estilo,
                        StockActual = p.StockActual
                    })
                    .OrderBy(p => p.Estilo)
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
                Nombre = dto.Nombre.Trim(),
                CapacidadLitros = dto.CapacidadLitros
            };
            await _formatoRepository.AddAsync(formato);

            // Crear entrada de stock por cada estilo existente en recetas
            var recetas = await _recetaRepository.GetAllAsync();
            var estilosDistintos = recetas
                .Select(r => r.Estilo)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToList();

            foreach (var estilo in estilosDistintos)
            {
                await _productoStockRepository.AddAsync(new ProductoStock
                {
                    FormatoEnvaseId = formato.Id,
                    Estilo = estilo!,
                    StockActual = 0
                });
            }

            var productos = estilosDistintos.Select((e, i) => new ProductoStockDto
            {
                Estilo = e!,
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
            var tieneDesignaciones = await _designacionRepository.CountAsync(d => d.FormatoEnvaseId == id) > 0;
            if (tieneDesignaciones)
                throw new InvalidOperationException(
                    "No se puede eliminar: hay lotes que usan este formato en sus designaciones de volumen.");

            var productos = await _productoStockRepository.FindAllAsync();
            var productosDelFormato = productos.Where(p => p.FormatoEnvaseId == id).ToList();

            foreach (var p in productosDelFormato)
                _productoStockRepository.Remove(p.Id);

            _formatoRepository.Remove(id);
            return true;
        }

        public async Task AgregarEstiloATodosLosFormatosAsync(string estilo)
        {
            var formatos = await _formatoRepository.FindAllAsync();
            var productosExistentes = await _productoStockRepository.FindAllAsync();

            foreach (var formato in formatos)
            {
                bool yaExiste = productosExistentes.Any(p =>
                    p.FormatoEnvaseId == formato.Id &&
                    p.Estilo.Equals(estilo, StringComparison.OrdinalIgnoreCase));

                if (!yaExiste)
                {
                    await _productoStockRepository.AddAsync(new ProductoStock
                    {
                        FormatoEnvaseId = formato.Id,
                        Estilo = estilo,
                        StockActual = 0
                    });
                }
            }
        }
    }
}
