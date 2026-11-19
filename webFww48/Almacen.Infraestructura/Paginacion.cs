using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Infraestructura
{
    public class Paginacion<T>:PagedBase
    {
        public IEnumerable<T> Lista { get; set; }
    }
}
