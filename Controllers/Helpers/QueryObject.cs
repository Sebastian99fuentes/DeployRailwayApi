using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDeployReservas.Controllers.Helpers
{
    public class QueryObject
    {
        public string?  AreaNombre {get; set; } = null; 
        public string? SortBy {get; set;} = null;

         public string?  ImplementoNombre {get; set; } = null; 

        public bool IsDecsending {get; set; }  = false ; 

        public int PageNumber {get; set;} = 1;

        public int PageSize {get; set;} = 20;
        
    }
}