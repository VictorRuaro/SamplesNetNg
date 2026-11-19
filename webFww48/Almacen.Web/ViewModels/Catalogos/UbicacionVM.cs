using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class UbicacionVM : EntityBase
    {
        public int idUbicacion { get; set; }

        [Display(Name = "Ubicación")]
        [Required]
        public string Ubicacion { get; set; }
    }
}