using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class AlmacenajeVM : EntityBase
    {
        public int idAlmacenaje { get; set; }

        [Display(Name = "Almacenaje ")]
        public string Almacenaje { get; set; }
    }
}