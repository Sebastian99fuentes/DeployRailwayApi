using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Controllers.Dtos.Area;
using ApiDeployReservas.Data.Models;
using Npgsql.Replication;

namespace ApiDeployReservas.Controllers.Mappers
{
    public static class AreaMappers
    {
        public static AreasDto ToAreasDto (this Area AreaModel)
        {
            return new AreasDto
            {
                 Id = AreaModel.Id,
                Nombre = AreaModel.Nombre,
                Ubicacion = AreaModel.Ubicacion,
                Descripcion = AreaModel.Descripcion

            };
        }

        public static AreaDto ToAreaDto (this Area AreaModel)
        {
            return new AreaDto
            {
                Id = AreaModel.Id,
                Nombre = AreaModel.Nombre,
                Ubicacion = AreaModel.Ubicacion,
                Descripcion = AreaModel.Descripcion,

            };
        } 

        public static Area ToAreaFromCreateDto(this  CreateAreaRequestDto areaDto)
        {
            return new Area
            {
                 Nombre = areaDto.Nombre,
                 Ubicacion = areaDto.Ubicacion,
                 Descripcion = areaDto.Descripcion

            };
        }
    }
} 

