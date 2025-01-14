using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Controllers.Dtos.Implemento;
using ApiDeployReservas.Controllers.Helpers;
using ApiDeployReservas.Data.Models;

namespace ApiDeployReservas.Repository.Interfaces
{
    public interface IImplementosRepository
    {
        Task <List<Implemento>>  GetallAsync(); 

        Task<Implemento?> GetByIdAsync(Guid id);

        Task <Implemento?> CreateAsync(Implemento implemento);

        Task <Implemento?> UpdateAsync(Guid id, CreateImplementoRequestDto implementoDto); 

        Task <Implemento?> UpdateImpleAsync(Guid? id, bool upDown);

        Task<Implemento?> DeleteAsync(Guid id);
        Task<bool> Exist(Guid id);  
        
    }
}