using Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamenMVC.Models
{
    public class ModelProductos
    {
        public string id { get; set; }
        public string Descripcion { get; set; }

        public List<Productos> Lista { get; set; }
    }
}