using AtonBeerTesis.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IStockService
    {
        Task<bool>RegistrarMovimientoStockAsync(MovimientoStockDto dto);
    }
}
