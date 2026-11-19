using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class MaterialDetalleSustancia
    {
        public int idMaterialDetalleSustancia { get; set; }

        [Display(Name = "Tipo de Sal ")]
        public string idTipoSal { get; set; }
    }
}