namespace Almacen.Infraestructura
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class EntityBase: ICamposControl
    {
        [Display(Name = "Usuario Registro")]
        public string UG { get; set; }
        [Display(Name = "Fecha Registro")]
        public DateTime FG { get; set; }
        [Display(Name = "Usuario Modificación")]
        public string UM { get; set; }
        [Display(Name = "Fecha Modificación")]
        public DateTime? FM { get; set; }
        [Display(Name = "Usuario Baja")]
        public string UB { get; set; }
        [Display(Name = "Fecha Baja")]
        public DateTime? FB { get; set; }
        [Display(Name = "Activo")]
        public bool ST { get; set; }
    }
}
