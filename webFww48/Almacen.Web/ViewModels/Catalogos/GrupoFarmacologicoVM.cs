using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class GrupoFarmacologicoVM : EntityBase
    {
        [Display(Name = "Grupo Farmacológico")]
        public int idGrupoFarmacologico { get; set; }

        [Display(Name = "Grupo Farmacológico")]
        [StringLength(150, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 4)]
        [Required]
        public string GrupoFarmacologico { get; set; }

        [Display(Name = "Color")]
        [StringLength(50, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 4)]

        public string color { get; set; }

        [Display(Name = "Color HTML")]
        [UIHint("UIColorPicker")]
        public string colorHtml { get; set; }
    }
}