using Almacen.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Almacen.Web.Aplicacion
{
    public class RespuestaFlujo
    {
        public ValorRespuesta Status { get; set; }

        public bool ValueSuccess { get; set; }

        public string Mensaje { get; set; }

        public string tipoMensaje { get; set; }

        public bool visibleBtnEditar { get; set; }

        public bool visibleBtnEliminar { get; set; }

        public bool visibleBtnGuardar { get; set; }

        public bool visibleBtnEnviar { get; set; }

        public bool visibleBtnAutorizar { get; set; }

        public bool visibleBtnDescarga { get; set; }

        public bool visibleBtnRegresarSinEnviar { get; set; }

        public bool visibleBtnGenerarSalida { get; set; }

        //public Usuario oUsuario { get; set; }

    }

    public enum ValorRespuesta : int
    {
        /// <summary>
        /// Completo y sin error.
        /// </summary>
        Completo = 0,

        /// <summary>
        /// No completo con error.
        /// </summary>
        NoCompleto = 1

    }
}