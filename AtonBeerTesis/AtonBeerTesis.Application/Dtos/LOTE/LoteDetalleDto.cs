namespace AtonBeerTesis.Application.DTOs
{
    public class LoteDetalleDto : LoteDto
    {
        public int CantidadRegistros { get; set; }
        public decimal? UltimoPh { get; set; }
        public decimal? UltimaDensidad { get; set; }
        public decimal? UltimaTemperatura { get; set; }
        public decimal? UltimaPresion { get; set; }
    }
}