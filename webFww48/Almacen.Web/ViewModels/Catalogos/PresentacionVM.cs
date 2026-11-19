using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class PresentacionVM : EntityBase
    {
        public int idPresentacion { get; set; }

        [Display(Name = "Presentacion")]
        [Required]
        public string Presentacion { get; set; }
    }
}