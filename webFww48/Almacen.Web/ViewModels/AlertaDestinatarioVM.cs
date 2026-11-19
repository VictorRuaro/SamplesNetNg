using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels
{
    public class AlertaDestinatarioVM : EntityBase
    {

        public AlertaDestinatarioVM()
        {
            this.Rol = new RolesVM();
        }

        public int idAlertaDestinatario { get; set; }

        [Display(Name = "Alerta")]
        [Required(ErrorMessage = "La Alerta es requerida")]
        public int idAlerta { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El Rol es requerido")]
        public string idRol { get; set; }

        [Display(Name = "Alerta")]
        public Alerta tAlerta { get; set; }

        [Display(Name = "Usuario")]
        public RolesVM Rol { get; set; }




    }
}