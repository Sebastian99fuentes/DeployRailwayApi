using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDeployReservas.Controllers.Dtos.ReservaImplemento
{
    public class ReservaImplementoDto
    {
     public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;

    public DateTime Start { get; set; }
     public DateTime End { get; set; }
    }
}