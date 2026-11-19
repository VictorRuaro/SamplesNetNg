using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class UMConcentracionVM : EntityBase
    {
        public int idUMConcentracion { get; set; }

        [Display(Name = "Unidad de Medida Concentración")]
        [Required]
        public string UMConcentracion { get; set; }
    }
}