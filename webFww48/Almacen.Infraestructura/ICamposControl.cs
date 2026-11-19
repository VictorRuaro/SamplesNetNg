using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Infraestructura
{
    interface ICamposControl
    {
        string UG { get; set; }
        DateTime FG { get; set; }
        string UM { get; set; }
        DateTime? FM { get; set; }
        string UB { get; set; }
        DateTime? FB { get; set; }
        bool ST { get; set; }
    }
}
