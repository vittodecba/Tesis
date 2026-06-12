using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Domain.Entities
{
    // Datos del emisor (la fábrica de cerveza). Tabla de una sola fila (Id = 1).
    public class Empresa
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string DomicilioComercial { get; set; } = string.Empty;
        public CondicionIVA CondicionIVA { get; set; } = CondicionIVA.ResponsableInscripto;
        public int PuntoVenta { get; set; } = 1;
        public string IngresosBrutos { get; set; } = string.Empty;
        public DateTime InicioActividades { get; set; }
    }
}
