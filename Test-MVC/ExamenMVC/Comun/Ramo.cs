using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun
{
    public class Ramo
    {
        public string id { get; set; }
        public string Pais { get; set; }
        public string Descripcion { get; set; }

        public List<Productos> Productos { get; set; }
    }
}
