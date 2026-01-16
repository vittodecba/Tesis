using AtonBeerTesis.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerarTokenJWT(Usuario usuario);
    }
}
