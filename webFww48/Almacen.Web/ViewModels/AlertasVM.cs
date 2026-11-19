
using Almacen.Infraestructura;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class AlertasVM
    {

        [Display(Name = "Alerta Caducos")]
        public string Alertas { get; set; }

        [Display(Name = "Listado de Alertas")]
        public virtual List<Alerta> Lista { get; set; }

    }

    public class Alerta:EntityBase
    {
        public int idAlerta { get; set; }

        [Display(Name = "Descripción Alerta")]
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Display(Name = "Valor")]
        [Required]
        [StringLength(5)]
        public string Valor { get; set; }

        public Nullable<int> Valor100referencia { get; set; }

        [Display(Name = "Unidad")]
        public int idAlertaUnidad { get; set; }
        [Display(Name = "Calculo")]
        public string Calculo { get; set; }

        public virtual AlertaUnidadVM tAlertaUnidad { get; set; }
        
        public virtual ICollection<AlertaXSustanciaMaterialVM> tAlertaXSustanciaMaterial { get; set; }

    }

    public partial class AlertaUnidadVM : EntityBase
    {
        
        public AlertaUnidadVM()
        {
            this.tAlerta = new HashSet<Alerta>();
        }

        public int idAlertaUnidad { get; set; }

        [Display(Name = "Unidad")]
        public string AlertaUnidad { get; set; }

        public virtual ICollection<Alerta> tAlerta { get; set; }
    }

    public partial class AlertaXSustanciaMaterialVM : EntityBase
    {
        public int idAlertaXSustanciaMaterial { get; set; }
        public int idAlerta { get; set; }
        public int idCategoria { get; set; }
        public int idSustanciaMaterial { get; set; }

        [Display(Name = "Valor")]
        [Required]
        public int Valor { get; set; }

        [Display(Name = "Valor")]
        [Required]
        [StringLength(50)]

        public Nullable<int> Valor100referencia { get; set; }

        public string Comentario { get; set; }

        public virtual Alerta tAlerta { get; set; }
    }
}