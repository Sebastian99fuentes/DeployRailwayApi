using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ApiDeployReservas.Controllers.Dtos.Implemento;
using ApiDeployReservas.Data.Models;

namespace ApiDeployReservas.Controllers.Mappers
{
    public static class ImplementoMapper
    {
        public static ImplementosDto ToimplementosDto(this Implemento implementoModel)
        {
            return new ImplementosDto 
            {
                Id = implementoModel.Id,
                NombreImple = implementoModel.NombreImple,
                Cantidad = implementoModel.Cantidad
            };
        } 
        public static ImplementoDto ToImplementoDto (this Implemento implementoModel)
        {
            return new ImplementoDto
            {
                Id = implementoModel.Id,
                NombreImple = implementoModel.NombreImple,
                Cantidad = implementoModel.Cantidad,
               
            };
        }

        public static Implemento ToImplementoFromCreateDto (this CreateImplementoRequestDto implementoDto)
        {
            return new Implemento
            {
                NombreImple = implementoDto.NombreImple,
                Cantidad = implementoDto.Cantidad
            };
        }
    }
}