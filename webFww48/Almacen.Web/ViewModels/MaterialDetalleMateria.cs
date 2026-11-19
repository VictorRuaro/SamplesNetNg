using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Almacen.Web.ViewModels.Catalogos;

namespace Almacen.Web.ViewModels
{
    public class MaterialDetalleMateria
    {
        public int idMaterialDetalleMateria { get; set; }

        [Display(Name = "Almacenaje")]
        public string idAlmacenaje { get; set; }

        public virtual AlmacenajeVM Almacenaje { get; set; }
    }
}