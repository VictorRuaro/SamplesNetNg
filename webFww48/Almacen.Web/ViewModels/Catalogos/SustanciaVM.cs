using Almacen.Infraestructura;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Catalogos
{
    public class SustanciaVM : EntityBase
    {
        public int idSustanciaMaterial { get; set; }

        [Display(Name = "Código")]
        [Required]
        public string Codigo { get; set; }

        [Display(Name = "Sustancia")]
        [Required]
        public string Descripcion { get; set; }

        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Display(Name = "Categoria")]
        public CategoriaVM cCategoria { get; set; }

        [Display(Name = "Sinonimos")]
        public string Sinonimos { get; set; }

        [Display(Name = "CAS")]
        public string CAS { get; set; }

        [Display(Name = "PM Compuesto")]
        public string PMCompuesto { get; set; }

        [Display(Name = "Formula Molecular")]
        public string FormulaMolecular { get; set; }

        [Display(Name = "Sustancia Controlada")]
        //[UIHint("SiNo")]
        public bool EsSustanciaControlada { get; set; }

        [Display(Name = "Grupo Farmacológico")]
        public int idGrupoFarmacologico { get; set; }

        [Display(Name = "Grupo Farmacológico")]
        public virtual GrupoFarmacologicoVM cGrupoFarmacologico { get; set; }

        public virtual List<AlmacenVM> tAlmacen { get; set; }

    }

    public class SustanciaFiltrosVM
    {
        public int idSustanciaMaterial { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Sustancia")]
        public string Descripcion { get; set; }

        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Display(Name = "Sinonimos")]
        public string Sinonimos { get; set; }

        [Display(Name = "CAS")]
        public string CAS { get; set; }

        [Display(Name = "PM Compuesto")]
        public string PMCompuesto { get; set; }

        [Display(Name = "Formula Molecular")]
        public string FormulaMolecular { get; set; }

        [Display(Name = "Sustancia Controlada")]
        [UIHint("UISiNo")]
        public bool? EsSustanciaControlada { get; set; }

        [Display(Name = "Grupo Farmacológico")]
        public int? idGrupoFarmacologico { get; set; }

    }
}