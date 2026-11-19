using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class MarcaVM : EntityBase
    {
        public int idMarca { get; set; }

        [Display(Name = "Marca")]
        [Required]
        public string Marca { get; set; }
    }
}