using AtonBeerTesis.Application.Dtos.STOCK;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface ILoteDesignacionService
    {
        Task<IEnumerable<LoteDesignacionDto>> ObtenerPorLoteAsync(int loteId);
        Task<LoteDesignacionDto> AgregarDesignacionAsync(int loteId, CreateLoteDesignacionDto dto);
        Task<bool> EliminarDesignacionAsync(int id);
    }
}
