using Almacen.Infraestructura;
using System;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class EstatusSolicitudVM : EntityBase
    {
        public EstatusSolicitudVM()
        {
            //this.tSolicitud = new HashSet<tSolicitud>();
        }
        public int idEstatusSolicitud { get; set; }
        public Nullable<int> NumEstatus { get; set; }
        
        [Display(Name = "Estatus Solicitud ")]
        public string EstatusSolicitud { get; set; }

        //public virtual ICollection<tSolicitud> tSolicitud { get; set; }
    }
}