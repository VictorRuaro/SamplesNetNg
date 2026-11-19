using Almacen.Infraestructura;
using Almacen.Web.CustomAnnotations;
using Almacen.Web.ViewModels.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class AlmacenSustanciaVM : AlmacenVM
    {
        [Display(Name = "Fabricante")]
        [Required(ErrorMessage = "El Fabricante es requerido")]
        public int idProveedor { get; set; }

        [Display(Name = "Fabricante")]
        public virtual ProveedorVM cProveedor { get; set; }

        [Display(Name = "Sustancia")]
        [Required(ErrorMessage = "La Sustancia es requerida")]
        public override int idSustanciaMaterial { get; set; }

        [Display(Name = "Sustancia")]
        public virtual SustanciaVM tSustanciaMaterial { get; set; }

        //Propiedades Sustancia

        [Display(Name = "Tipo de Sal")]
        [Required(ErrorMessage = "El Tipo de Sal es requerido")]
        public int idTipoSal { get; set; }

        [Display(Name = "Referencia")]
        [MaxLength(50, ErrorMessage = "El numero máximo de caracteres es de 50")]
        public string Referencia { get; set; }

        [Display(Name = "Conservación")]
        [Required(ErrorMessage = "La Conservación es requerida")]
        public int idConservacion { get; set; }

        [Display(Name = "Ubicación")]
        //[Required(ErrorMessage = "La Ubicación es requerida")]
        public int idUbicacion { get; set; }

        [Display(Name = "Tipo de Envase")]
        [Required(ErrorMessage = "El Tipo de Envase es requerido")]
        public int idTipoEnvase { get; set; }

        [Display(Name = "No. Envases")]
        [Required(ErrorMessage = "El No. Envases es requerido")]
        [Range(1, 99999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        public override int Existencia { get; set; }

        [Display(Name = "Cantidad por Envase")]
        [Required(ErrorMessage = "La Cantidad por Envase es requerida")]
        [Range(1, 9999999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        public int CantidadPorEnvase { get; set; }

        [Display(Name = "Concentración")]
        [RequiredIf("idUMConcentracion", null, ErrorMessage = "La Concentración es requerida", IsInverted = true)]
        public Nullable<int> Concentracion { get; set; }

        [Display(Name = "Disolvente")]
        //[Required(ErrorMessage = "El Disolvente es requerido")]
        public int idDisolvente { get; set; }

        [Display(Name = "UM. Cantidad por Envase")]
        [Required(ErrorMessage = "La UM. Cantidad por Envase es requerida")]
        public int idUMCantidadEnvase { get; set; }

        [Display(Name = "UM. Concentración")]
        [RequiredIf("Concentracion",null,ErrorMessage = "La UM. Concentración es requerida",IsInverted =true)]
        public Nullable<int> idUMConcentracion { get; set; }

        public virtual ConservacionVM cConservacion { get; set; }
        public virtual DisolventeVM cDisolvente { get; set; }
        public virtual TipoEnvaseVM cTipoEnvase { get; set; }
        public virtual TipoSalVM cTipoSal { get; set; }
        public virtual UbicacionVM cUbicacion { get; set; }
        public virtual UMCantidadEnvaseVM cUMCantidadEnvase { get; set; }
        public virtual UMConcentracionVM cUMConcentracion { get; set; }

        
    }
}