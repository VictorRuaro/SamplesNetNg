using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class DisolventeVM : EntityBase
    {
        public int idDisolvente { get; set; }

        [Display(Name = "Disolvente")]
        [Required]
        public string Disolvente { get; set; }
    }
}