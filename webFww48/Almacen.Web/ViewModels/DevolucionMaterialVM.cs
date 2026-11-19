using Almacen.Infraestructura;
using Almacen.Web.ViewModels.Catalogos;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels
{
    public class DevolucionMaterialVM : DevolucionVM
    {
        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "El Proveedor es requerido")]
        public int idProveedor { get; set; }

        [Display(Name = "Proveedor")]
        public virtual ProveedorVM cProveedor { get; set; }

        [Display(Name = "Material")]
        [Required(ErrorMessage = "El Material es requerido")]
        public override int idSustanciaMaterial { get; set; }

        [Display(Name = "Material")]
        public virtual MaterialVM tSustanciaMaterial { get; set; }

        //Propiedades Material

        [Display(Name = "No. de Parte")]
        [MaxLength(20, ErrorMessage = "El numero máximo de caracteres es de 20")]
        public string NumParte { get; set; }

        [Display(Name = "No. de Catalogo")]
        [MaxLength(20, ErrorMessage = "El numero máximo de caracteres es de 20")]
        public string NumCatalago { get; set; }

        [Display(Name = "Unidades Recibidas")]
        [Required(ErrorMessage = "Las Unidades Recibidas son requeridas")]
        [Range(1, 99999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        public override int Existencia { get; set; }

        [Display(Name = "Marca")]
        [Required(ErrorMessage = "La Marca es requerida")]
        public int idMarca { get; set; }

        [Display(Name = "Condiciones de Almacenaje")]
        [Required(ErrorMessage = "Las Condiciones de Almacenaje es requerido")]
        public int idCondicionesAlmacenaje { get; set; }

        [Display(Name = "Presentación")]
        [Required(ErrorMessage = "La Presentación es requerida")]
        public int idPresentacion { get; set; }


        public virtual MarcaVM cMarca { get; set; }
        public virtual CondicionesAlmacenajeVM cCondicionesAlmacenaje { get; set; }
        public virtual PresentacionVM cPresentacion { get; set; }
    }
}