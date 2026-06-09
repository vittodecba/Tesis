using AtonBeerTesis.Application.Dtos.EMPRESA;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;

        public EmpresaService(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        public async Task<EmpresaDto?> ObtenerAsync()
        {
            var e = await _empresaRepository.GetAsync();
            if (e is null) return null;

            return new EmpresaDto
            {
                Id                 = e.Id,
                RazonSocial        = e.RazonSocial,
                Cuit               = e.Cuit,
                DomicilioComercial = e.DomicilioComercial,
                CondicionIVA       = e.CondicionIVA.ToString(),
                PuntoVenta         = e.PuntoVenta,
                IngresosBrutos     = e.IngresosBrutos,
                InicioActividades  = e.InicioActividades
            };
        }

        public async Task<bool> ActualizarAsync(ActualizarEmpresaDto dto)
        {
            var empresa = await _empresaRepository.GetAsync();
            if (empresa is null) return false;

            empresa.RazonSocial        = dto.RazonSocial;
            empresa.Cuit               = dto.Cuit;
            empresa.DomicilioComercial = dto.DomicilioComercial;
            empresa.PuntoVenta         = dto.PuntoVenta;
            empresa.IngresosBrutos     = dto.IngresosBrutos;
            empresa.InicioActividades  = dto.InicioActividades;

            if (!string.IsNullOrWhiteSpace(dto.CondicionIVA))
            {
                if (!Enum.TryParse<CondicionIVA>(dto.CondicionIVA, true, out var cond))
                    throw new Exception("Condición de IVA inválida");
                empresa.CondicionIVA = cond;
            }

            await _empresaRepository.UpdateAsync(empresa);
            return true;
        }
    }
}
