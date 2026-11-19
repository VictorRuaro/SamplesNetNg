using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Almacen.Infraestructura;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels
{
    public class AlmacenCertificadoVM: EntityBase
    {
        public int idAlmacenCertificado { get; set; }

        public int idAlmacen { get; set; }

        public string NombreArchivo { get; set; }

        [Display(Name = "Nueva Fecha Caducidad")]
        [Required(ErrorMessage = "La Nueva Fecha Caducidad es requerida")]
        public Nullable<System.DateTime> FechaCaducidad { get; set; }

        [FileTypes("jpg,jpeg,png,pdf")]
        [FileSize(5120000)]
        [Display(Name = "Nuevo Certificado")]
        [Required(ErrorMessage = "El Certificado es requerido")]
        public HttpPostedFileBase Upload { get; set; }

        public byte[] Archivo { get; set; }

        public string Observaciones { get; set; }

        public virtual AlmacenVM tAlmacen { get; set; }
    }
}