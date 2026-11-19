using Almacen.Infraestructura;
using System;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class ProveedorVM : EntityBase
    {
        public int idProveedor { get; set; }

        [Display(Name = "Proveedor")]
        [Required]
        public string Proveedor { get; set; }

        [Display(Name = "Categoria")]
        public Nullable<int> idCategoria { get; set; }
    }
}