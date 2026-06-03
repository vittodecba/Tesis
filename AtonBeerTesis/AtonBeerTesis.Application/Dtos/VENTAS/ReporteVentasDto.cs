namespace AtonBeerBackend.Models.DTOs
{
    public class ReporteVentasDto
    {
        public decimal TotalVendido { get; set; }
        public int CantidadVentas { get; set; }
        public decimal EfectivoTotal { get; set; }
        public decimal TransferenciaTotal { get; set; }
        public decimal TicketPromedio { get; set; }
        public decimal VariacionIngresosPorcentaje { get; set; }
        public List<VentaPorDiaDto> VentasPorDia { get; set; } = new List<VentaPorDiaDto>();
        public List<TopClienteDto> TopClientes { get; set; } = new List<TopClienteDto>();
        public List<TopProductoDto> TopProductos { get; set; } = new List<TopProductoDto>();
        public List<TopEstiloDto> TopEstilos { get; set; } = new List<TopEstiloDto>();
    }

    public class VentaPorDiaDto
    {
        public string Fecha { get; set; }
        public decimal Total { get; set; }
    }

    public class TopClienteDto
    {
        public string Cliente { get; set; }
        public decimal TotalComprado { get; set; }
        public int CantidadVentas { get; set; }
    }

    public class TopProductoDto
    {
        public string Producto { get; set; }
        public int CantidadVendida { get; set; }
    }

    public class TopEstiloDto
    {
        public string Estilo { get; set; }
        public int CantidadVendida { get; set; }
    }
}