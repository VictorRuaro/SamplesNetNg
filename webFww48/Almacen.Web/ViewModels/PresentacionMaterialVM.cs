using Almacen.Web.ViewModels.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class PresentacionMaterialVM
    {
        public int idSustanciaMaterial { get; set; }

        // Almacen
        public int idAlmacen { get; set; }

        [Display(Name = "Unidades Recibidas")]
        public int Existencia { get; set; }

        public int idTipoEnvase { get; set; }

        [Display(Name = "Presentación")]
        public int idPresentacion { get; set; }

        public virtual PresentacionVM cPresentacion { get; set; }

    }
}