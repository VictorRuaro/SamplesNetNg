using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Almacen.Web.ViewModels.Catalogos;
using Almacen.Infraestructura;
using Almacen.Web.Aplicacion;
using Almacen.Web.CustomAnnotations;

namespace Almacen.Web.ViewModels
{
    public class SolicitudVM: EntityBase
    {
        [Display(Name = "Solicitud")]
        public int idSolicitud { get; set; }

        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Display(Name = "Folio")]
        public int Folio { get; set; }

        [Display(Name = "Descripción Solicitud")]
        [Required(ErrorMessage = "La {0} es obligatoria.")]
        public string Descripcion { get; set; }

        [Display(Name = "Descripción de uso")]
        public string DescripcionUso { get; set; }

        [Display(Name = "Solicitante")]
        public string idSolicitante { get; set; }

        [Display(Name = "Solicitante")]
        public Usuario Solicitante { get; set; }

        [Display(Name = "Estatus Solicitud")]
        public int idEstatusSolicitud { get; set; }

        [Display(Name = "Autorizador")]
        public string idAutorizador { get; set; }

        [Display(Name = "Fecha Envio")]
        public Nullable<System.DateTime> FechaEnvio { get; set; }

        [Display(Name = "Fecha Autoriza")]
        public Nullable<System.DateTime> FechaAutoriza { get; set; }

        public string idSurtidor { get; set; }

        [Display(Name = "Fecha Surtida")]
        public Nullable<System.DateTime> FechaSurtida { get; set; }

        [Display(Name = "Acción Correctiva")]
        public string AccionCorrectiva { get; set; }

        [Display(Name = "Autorizador")]
        public Usuario Autorizador { get; set; }

        [Display(Name = "Categoria")]
        public virtual CategoriaVM Categoria { get; set; }

        [Display(Name = "Estatus")]
        public virtual EstatusSolicitudVM cEstatusSolicitud { get; set; }

        public virtual SolicitudDetalleVM Model { get; set; }
        public virtual List<SolicitudDetalleVM> Materiales { get; set; }

        public virtual RespuestaFlujo ValidaFlujo { get; set; }

        public bool SolicitudEditable { get; set; }


        [Display(Name = "Firma del Solicitante")]
        [RequiredIf("idAutorizador", null, ErrorMessage = "Debe ingresar la Firma del Solicitante", IsInverted = true)]
        public string FirmaSolicitante { get; set; }

        [Display(Name = "Fecha Devolución")]
        public Nullable<System.DateTime> FechaDevolucion { get; set; }

        [Display(Name = "Usuario Devolución")]
        public string idUsuarioDevolucion { get; set; }


    }

    public class SolicitudFiltrosVM
    {
        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Display(Name = "Folio")]
        public int? Folio { get; set; }

        [Display(Name = "Descripción Solicitud")]
        public string Descripcion { get; set; }

        [Display(Name = "Descripción de uso")]
        public string DescripcionUso { get; set; }

        [Display(Name = "Solicitante")]
        public string idSolicitante { get; set; }

        [Display(Name = "Solicitante")]
        public Usuario Solicitante { get; set; }

        [Display(Name = "Estatus Solicitud")]
        public int? idEstatusSolicitud { get; set; }

        [Display(Name = "Autorizador")]
        public string idAutorizador { get; set; }

        [Display(Name = "Accion Correctiva")]
        public string AccionCorrectiva { get; set; }

        //se agregan filtros modulo Autorizacion
        [Display(Name = "Mostrar Autorizadas")]
        public bool EstatusAutorizadas { get; set; }


        //se agregan filtros modulo Surtidas
        [Display(Name = "Mostrar Surtidas")]
        public bool EstatusSurtidas { get; set; }

        //se agregan filtros modulo Devolucion
        [Display(Name = "Mostrar Surtidas con Devolución ")]
        public bool EstatusSurtidasDevolucion { get; set; }

        public virtual SolicitudDetalleVM Materiales { get; set; }

    }
}