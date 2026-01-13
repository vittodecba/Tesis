using AtonBeerTesis.Application.Common;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dtos.Cliente;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<List<ClienteDto>> GetAllAsync(string? tipo = null, string? ubicacion = null, string? estado = null)
        {
            var clientes = await _clienteRepository.GetAllAsync();

            // Estado: por defecto solo Activos
            if (!string.IsNullOrWhiteSpace(estado) &&
                Enum.TryParse<EstadoCliente>(estado, true, out var estadoEnum))
            {
                clientes = clientes.Where(c => c.EstadoCliente == estadoEnum).ToList();
            }
            else
            {
                clientes = clientes.Where(c => c.EstadoCliente == EstadoCliente.Activo).ToList();
            }

            // Tipo
            if (!string.IsNullOrWhiteSpace(tipo) &&
                Enum.TryParse<TipoCliente>(tipo, true, out var tipoEnum))
            {
                clientes = clientes.Where(c => c.Tipocliente == tipoEnum).ToList();
            }

            // Ubicacion
            if (!string.IsNullOrWhiteSpace(ubicacion))
            {
                clientes = clientes
                    .Where(c => c.Ubicacion.Contains(ubicacion, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return clientes.Select(MapToDto).ToList();
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            return cliente is null ? null : MapToDto(cliente);
        }

        public async Task<int> CreateAsync(CrearClienteDto dto)
        {
            var cuit = CuitValidator.Normalize(dto.Cuit);

            if (!CuitValidator.IsValid(cuit))
                throw new Exception("El CUIT ingresado no es válido");

            if (await ExisteCuitAsync(cuit))
                throw new Exception("Ya existe un cliente con ese CUIT");

            if (!Enum.TryParse<TipoCliente>(dto.TipoCliente, true, out var tipo))
                throw new Exception("Tipo de cliente inválido");

            var cliente = new Cliente
            {
                RazonSocial = dto.RazonSocial,
                Cuit = cuit, // ← guardado normalizado
                Tipocliente = tipo,
                EstadoCliente = EstadoCliente.Activo,
                Email = dto.Email,
                Ubicacion = dto.Ubicacion,
                ContactoNombre = dto.ContactoNombre,
                ContactoTelefono = dto.ContactoTelefono,
                ContactoEmail = dto.ContactoEmail,
                TotalPedidos = 0
            };

            await _clienteRepository.AddAsync(cliente);
            return cliente.IdCliente;
        }

        public async Task<bool> UpdateAsync(int id, ActualizarClienteDto dto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null) return false;

            var cuit = CuitValidator.Normalize(dto.Cuit);

            if (!CuitValidator.IsValid(cuit))
                throw new Exception("El CUIT ingresado no es válido");

            if (await ExisteCuitAsync(cuit, id))
                throw new Exception("Ya existe otro cliente con ese CUIT");

            if (!Enum.TryParse<TipoCliente>(dto.TipoCliente, true, out var tipo))
                throw new Exception("Tipo de cliente inválido");

            if (!Enum.TryParse<EstadoCliente>(dto.EstadoCliente, true, out var estado))
                throw new Exception("Estado de cliente inválido");

            cliente.RazonSocial = dto.RazonSocial;
            cliente.Cuit = cuit;
            cliente.Tipocliente = tipo;
            cliente.EstadoCliente = estado;
            cliente.Email = dto.Email;
            cliente.Ubicacion = dto.Ubicacion;
            cliente.ContactoNombre = dto.ContactoNombre;
            cliente.ContactoTelefono = dto.ContactoTelefono;
            cliente.ContactoEmail = dto.ContactoEmail;

            await _clienteRepository.UpdateAsync(cliente);
            return true;
        }
        public async Task<bool> PatchAsync(int id, PatchClienteDto dto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null) return false;

            // Strings simples
            if (dto.RazonSocial is not null) cliente.RazonSocial = dto.RazonSocial;
            if (dto.Email is not null) cliente.Email = dto.Email;
            if (dto.Ubicacion is not null) cliente.Ubicacion = dto.Ubicacion;

            if (dto.ContactoNombre is not null) cliente.ContactoNombre = dto.ContactoNombre;
            if (dto.ContactoTelefono is not null) cliente.ContactoTelefono = dto.ContactoTelefono;
            if (dto.ContactoEmail is not null) cliente.ContactoEmail = dto.ContactoEmail;

            // Enums (solo si vienen)
            if (dto.TipoCliente is not null)
            {
                if (!Enum.TryParse<TipoCliente>(dto.TipoCliente, true, out var tipo))
                    throw new Exception("Tipo de cliente inválido");

                cliente.Tipocliente = tipo; // OJO: en tu entidad es Tipocliente
            }

            if (dto.EstadoCliente is not null)
            {
                if (!Enum.TryParse<EstadoCliente>(dto.EstadoCliente, true, out var estado))
                    throw new Exception("Estado de cliente inválido");

                cliente.EstadoCliente = estado;
            }

            await _clienteRepository.UpdateAsync(cliente);
            return true;
        }


        public async Task<bool> DeactivateAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null) return false;

            cliente.EstadoCliente = EstadoCliente.Inactivo;
            await _clienteRepository.UpdateAsync(cliente);

            return true;
        }

        public List<string> GetTiposCliente() => Enum.GetNames(typeof(TipoCliente)).ToList();
        public List<string> GetEstadosCliente() => Enum.GetNames(typeof(EstadoCliente)).ToList();
        private async Task<bool> ExisteCuitAsync(string cuit, int? excluirId = null)
        {
            var clientes = await _clienteRepository.GetAllAsync();

            return clientes.Any(c =>
                c.Cuit == cuit &&
                (!excluirId.HasValue || c.IdCliente != excluirId.Value)
            );
        }
        private static ClienteDto MapToDto(Cliente c) => new ClienteDto
        {
            IdCliente = c.IdCliente,
            RazonSocial = c.RazonSocial,
            Cuit = c.Cuit,
            TipoCliente = c.Tipocliente.ToString(),
            Ubicacion = c.Ubicacion,

            ContactoNombre = c.ContactoNombre,
            ContactoTelefono = c.ContactoTelefono,

            Email = c.Email,
            ContactoEmail = c.ContactoEmail,

            UltimaCompra = c.UltimaCompra,
            EstadoCliente = c.EstadoCliente.ToString()
        };
    }
}
