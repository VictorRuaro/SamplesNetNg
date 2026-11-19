using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class CondicionesAlmacenajeVM : EntityBase
    {
        public int idCondicionesAlmacenaje { get; set; }
            
        [Display(Name = "Condiciones de Almacenaje")]
        [Required]
        public string CondicionesAlmacenaje { get; set; }
    }
}