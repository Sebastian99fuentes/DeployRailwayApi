using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Data.Models;

namespace ApiDeployReservas.Repository.Interfaces
{
    public interface ItokenService
    {
         string CreateToken(AppUser user);
    }
}