using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Controllers.Dtos.Area;
using ApiDeployReservas.Controllers.Helpers;
using ApiDeployReservas.Data.Models;

namespace ApiDeployReservas.Repository.Interfaces
{
    public interface IAreaRepository
    {

        Task<List<Area>> GetAllAsync(QueryObject query);

        Task<Area?> GetByIdAsync(Guid id);  ///first or default cant be null

        Task<Area> CreateAsync(Area area);

        Task<Area?> UpdateAsync(Guid id, CreateAreaRequestDto areaDto);

        Task<Area?> DeleteAsync(Guid id);

        Task<bool> Exist(Guid id);

    }
}