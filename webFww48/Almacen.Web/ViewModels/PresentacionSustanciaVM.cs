using Almacen.Web.ViewModels.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class PresentacionSustanciaVM
    {
        public int idSustanciaMaterial { get; set; }

        // Almacen
        public int idAlmacen { get; set; }

        [Display(Name = "No. Envases")]
        public int Existencia { get; set; }

        public int idTipoEnvase { get; set; }

        [Display(Name = "Cantidad por Envase")]
        public int CantidadPorEnvase { get; set; }

        [Display(Name = "UM. Cantidad por Envase")]
        public int idUMCantidadEnvase { get; set; }

        //[Display(Name = "Concentración")]
        //public string Concentracion { get; set; }

        //[Display(Name = "UM. Concentración")]
        //public int idUMConcentracion { get; set; }

        public virtual TipoEnvaseVM cTipoEnvase { get; set; }
        public virtual UMCantidadEnvaseVM cUMCantidadEnvase { get; set; }
        //public virtual UMConcentracionVM cUMConcentracion { get; set; }


    }
}