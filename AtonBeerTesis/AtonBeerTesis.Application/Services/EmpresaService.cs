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

            // CondicionIVA y PuntoVenta son fijos (Responsable Inscripto, PV 1): no se editan
            // desde la UI, se preservan tal cual están en la entidad.
            empresa.RazonSocial        = dto.RazonSocial;
            empresa.Cuit               = dto.Cuit;
            empresa.DomicilioComercial = dto.DomicilioComercial;
            empresa.InicioActividades  = dto.InicioActividades;

            await _empresaRepository.UpdateAsync(empresa);
            return true;
        }
    }
}
