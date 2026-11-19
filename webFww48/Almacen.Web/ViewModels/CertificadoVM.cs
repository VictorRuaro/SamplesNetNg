using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class CertificadoVM
    {
        public AlmacenSustanciaVM Sustancia { get; set; }
        public AlmacenCertificadoVM Certificado { get; set; }

        [Display(Name = "Historial de Certificados")]
        public List<AlmacenCertificadoVM> Historial { get; set; }
    }
}