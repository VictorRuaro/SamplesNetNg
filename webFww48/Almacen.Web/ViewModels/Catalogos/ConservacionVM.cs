using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class ConservacionVM : EntityBase
    {
        public int idConservacion { get; set; }

        [Display(Name ="Conservación")]
        [Required]
        public string Conservacion { get; set; }
    }
}