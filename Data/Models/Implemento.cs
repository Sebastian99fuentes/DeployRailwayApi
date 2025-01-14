using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Data.Models;

namespace ApiDeployReservas.Data.Models
{
    public class Implemento
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Esto generará un GUID automáticamente al crear un nuevo registro
        public string NombreImple { get; set; } = string.Empty;

        public int Cantidad { get; set; }
    }
}