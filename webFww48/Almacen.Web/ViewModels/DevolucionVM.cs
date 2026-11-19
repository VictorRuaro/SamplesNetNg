using Almacen.Infraestructura;
using System;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels
{
    public class DevolucionVM : EntityBase
    {
        public int idDevolucion { get; set; }
        public int idCategoria { get; set; }

        public virtual int idSustanciaMaterial { get; set; }

        [Display(Name = "Integrar al Almacen")]
        public bool IntegrarAlmacen { get; set; }

        [Display(Name = "No. de Lote Interno")]
        //[Required(ErrorMessage = "El No. de Lote Interno es requerido")]
        public int NumDeLoteInterno { get; set; }

        [Display(Name = "No. de Lote Interno")]
        public string LoteInterno { get; set; }

        public virtual int CantidadIngresa { get; set; }

        public virtual int Existencia { get; set; }

        [Display(Name = "Fecha Recepción")]
        //[Required(ErrorMessage = "La Fecha Recepción es requerida")]
        public Nullable<System.DateTime> FechaRecepcion { get; set; }

        [Display(Name = "Fecha Caducidad")]
        //[Required(ErrorMessage = "La Fecha Caducidad es requerida")]
        public Nullable<System.DateTime> FechaCaducidad { get; set; }

        [Display(Name = "Lote del Proveedor")]
        public string LoteProveedor { get; set; }

        [Display(Name = "Observaciones")]
        [MaxLength(150, ErrorMessage = "El numero máximo de caracteres es de 150")]
        public string Observaciones { get; set; }

        [Display(Name = "No. de Lote Interno")]
        [Required(ErrorMessage = "El Lote es requerido")]
        public virtual Nullable<int> idAlmacen { get; set; }

    }
}