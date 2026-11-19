using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class CategoriaVM : EntityBase
    {
        public int idCategoria { get; set; }

        [Display(Name = "Categoria")]
        [Required]
        public string Categoria { get; set; }
    }
}