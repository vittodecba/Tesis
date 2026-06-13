namespace AtonBeerTesis.Application.Dtos.EMPRESA
{
    public class EmpresaDto
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string DomicilioComercial { get; set; } = string.Empty;
        public string CondicionIVA { get; set; } = string.Empty;
        public int PuntoVenta { get; set; }
        public string IngresosBrutos { get; set; } = string.Empty;
        public DateTime InicioActividades { get; set; }
    }

    // Para actualizar los datos del emisor desde la UI de configuración.
    // CondicionIVA, PuntoVenta e IngresosBrutos no se editan desde la UI: no se incluyen acá.
    public class ActualizarEmpresaDto
    {
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string DomicilioComercial { get; set; } = string.Empty;
        public DateTime InicioActividades { get; set; }
    }
}
