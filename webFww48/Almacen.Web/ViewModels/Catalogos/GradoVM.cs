using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class GradoVM : EntityBase
    {
        public int idGrado { get; set; }

        [Display(Name = "Grado")]
        [Required]
        public string Grado { get; set; }
    }
}