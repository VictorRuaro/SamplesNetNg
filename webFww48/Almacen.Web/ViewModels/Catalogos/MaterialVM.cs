using Almacen.Infraestructura;
using Almacen.Web.ViewModels.Catalogos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class MaterialVM : EntityBase
    {
        public int idSustanciaMaterial { get; set; }

        [Display(Name = "ID")]
        [Required]
        public string Codigo { get; set; }

        [Display(Name = "Material")]
        [Required]
        public string Descripcion { get; set; }

        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Display(Name = "Categoria")]
        public CategoriaVM cCategoria { get; set; }

        [Display(Name = "Grado")]
        public int idGrado { get; set; }

        [Display(Name = "Grado")]
        public virtual GradoVM cGrado { get; set; }

        [Display(Name = "Numero de Parte")]
        public string NumeroParte { get; set; }

        public virtual List<AlmacenVM> tAlmacen { get; set; }
    }

    public class MaterialFiltrosVM
    {
        public int idSustanciaMaterial { get; set; }

        [Display(Name = "ID")]
        public string Codigo { get; set; }

        [Display(Name = "Material")]
        public string Descripcion { get; set; }

        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Display(Name = "Grado")]
        public int? idGrado { get; set; }

        [Display(Name = "Numero de Parte")]
        public string NumeroParte { get; set; }

    }
}