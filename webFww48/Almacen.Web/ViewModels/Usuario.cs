using Almacen.Infraestructura;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class Usuario: EntityBase
    {
        public string id{ get; set; }

        [Display(Name = "Nombre")]
        [StringLength(256)]
        public string NombreCompleto { get; set; }

        [Display(Name = "Correo")]
        [StringLength(256)]
        public string Email { get; set; }
    }
}



