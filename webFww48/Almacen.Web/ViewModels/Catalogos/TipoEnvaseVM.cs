using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class TipoEnvaseVM : EntityBase
    {
        public int idTipoEnvase { get; set; }

        [Display(Name = "TipoEnvase")]
        [Required]
        public string TipoEnvase { get; set; }
    }
}