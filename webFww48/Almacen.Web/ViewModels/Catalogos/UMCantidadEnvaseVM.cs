using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class UMCantidadEnvaseVM : EntityBase
    {
        public int idUMCantidadEnvase { get; set; }

        [Display(Name = "Unidad de Medida Cantidad por Envase")]
        [Required]
        public string UMCantidadEnvase { get; set; }
    }
}