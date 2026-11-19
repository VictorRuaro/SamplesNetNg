using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class TipoSalVM : EntityBase
    {
        public int idTipoSal { get; set; }

        [Display(Name = "Tipo de Sal")]
        [StringLength(50, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 4)]
        [Required]
        public string TipoSal { get; set; }
    }
}