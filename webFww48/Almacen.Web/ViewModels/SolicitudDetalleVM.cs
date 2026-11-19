using Almacen.Infraestructura;
using Almacen.Web.CustomAnnotations;
using Almacen.Web.ViewModels.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web.ViewModels
{
    public class SolicitudDetalleVM: EntityBase
    {
        public SolicitudDetalleVM()
        {
            this.tSolicitudDetalleLote = new List<SolicitudDetalleLoteVM>();
        }

        public int index { get; set; }

        public int idCategoria { get; set; }
        public int idSolicitudDetalle { get; set; }

        [Display(Name = "Material")]
        public int idSustanciaMaterial { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Solicitud")]
        public int idSolicitud { get; set; }
        

        [Display(Name = "Cantidad Solicitada")]
        [Required(ErrorMessage = "La {0} es obligatoria.")]
        [Range(1,99999,ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        [Existencias(action:"ObtenerExistencias",controller: "DatosAplicacion", AdditionalFields: "idAlmacen,idCategoria", ErrorMessage = "No se cuenta con la cantidad suficiente")]
        public int CantidadSolicitada { get; set; }

        [Display(Name = "Cantidad Autorizada")]
        [Required(ErrorMessage = "La {0} es obligatoria.")]
        [Range(1, 99999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        [ValueComparison("CantidadSolicitada",comparison: ValueComparison.IsLessThanOrEqual, NotValidateWhenValue:0,ErrorMessage = "La Cantidad Autorizada debe ser menor o igual a la Cantidad Solicitada")]
        public int CantidadAutorizada { get; set; }

        [Display(Name = "Cantidad Surtida")]
        //[Required(ErrorMessage = "La {0} es obligatoria.")]
        [Range(1, 99999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        //[ValueComparison("CantidadAutorizada", comparison: ValueComparison.IsLessThanOrEqual, NotValidateWhenValue: 0, ErrorMessage = "La Cantidad Surtida debe ser menor o igual a la Cantidad Autorizada")]
        public int CantidadSurtida { get; set; }

        [Display(Name = "Cantidad a Devolver")]
        
        [Range(0, 99999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        [ValueComparison("CantidadSurtida", comparison: ValueComparison.IsLessThan, NotValidateWhenValue: 0, ErrorMessage = "La Cantidad a Devolver debe ser menor a la Cantidad Surtida")]
        public int CantidadDevolver { get; set; }

        [Display(Name = "Total a Surtir")]
        //[Required(ErrorMessage = "La {0} es obligatoria.")]
        //[Range(1, 99999, ErrorMessage = "El {0} debe ser minimo de {1} y maximo {2}")]
        [ValueComparison("CantidadAutorizada", comparison: ValueComparison.IsLessThanOrEqual, NotValidateWhenValue: 0, ErrorMessage = "El Total a Surtir debe ser menor o igual a la Cantidad Autorizada")]
        public int TotalSurtida { get; set; }


        [Display(Name = "Cantidad Devuelta")]
        //[Range(0, 99999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        //[RequiredIf("CantidadDevolver", 0, ErrorMessage = "Debe ingresar la Cantidad Devuelta", IsInverted = true)]
        [ValueComparison("CantidadDevolver", comparison: ValueComparison.IsLessThanOrEqual, NotValidateWhenValue: 0, ErrorMessage = "La Cantidad a Devuelta debe ser menor o igual a la Cantidad Devolver")]
        public int CantidadDevuelta { get; set; }

        [Display(Name = "Estatus Material")]
        public int idSolicitudDetalleEstatus { get; set; }

        [Display(Name = "Estatus")]
        public int idEstatusSolicitud { get; set; }

        [Display(Name = "Material")]
        public MaterialVM Material { get; set; }

        [Display(Name = "Sustancia")]
        public SustanciaVM Sustancia { get; set; }

        [RequiredIf("CantidadSurtida", 0,ErrorMessage = "Debe seleccionar un lote",IsInverted =true)]
        public Nullable<int> idAlmacen { get; set; }

        [Display(Name = "Tipo de Envase")]
        public Nullable<int> idTipoEnvase { get; set; }

        [Display(Name = "Presentación")]
        [Required(ErrorMessage = "Debe seleccionar una {0}")]
        public Nullable<int> idPresentacion { get; set; }

        [Display(Name = "Unidad de Medida")]
        public Nullable<int> idUnidadMedida { get; set; }

        [ValueComparison("CantidadSurtida", comparison: ValueComparison.IsGreaterThanOrEqual, NotValidateWhenValue: 0, ErrorMessage = "La Cantidad Surtida no debe ser mayor con la del lote seleccionado")]
        public Nullable<int> Existencia { get; set; }

        public virtual List<SolicitudDetalleLoteVM> tSolicitudDetalleLote { get; set; }

    }

    public class SolicitudDetalleLoteVM : EntityBase
    {
        public int idSolicitudDetalleLote { get; set; }
        public int idSolicitudDetalle { get; set; }

        public int idAlmacen { get; set; }
        public int idSustanciaMaterial { get; set; }

        //public int NumDeLoteInterno { get; set; }

        //public string LoteInterno { get; set; }

        public int CantidadSurtida { get; set; }

        public int CantidadDevolver { get; set; }
        [Range(0, 99999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        [RequiredIf("CantidadDevolver", 0, ErrorMessage = "Debe ingresar la Cantidad Devuelta", IsInverted = true)]
        [ValueComparison("CantidadDevolver", comparison: ValueComparison.IsLessThanOrEqual, NotValidateWhenValue: 0, ErrorMessage = "La Cantidad a Devuelta debe ser menor o igual a la Cantidad Devolver")]
        public int CantidadDevuelta { get; set; }

        public virtual int Existencia { get; set; }


    }
}