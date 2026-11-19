using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Almacen.Web.ViewModels;
using Almacen.Web.ViewModels.Catalogos;

namespace Almacen.Web.Logica
{
    public static class DatosDemoSustancia
    {

        public static SolicitudVM SolicitudCreate()
        {
            SolicitudVM result = new SolicitudVM();
            result.Materiales = new List<SolicitudDetalleVM>();

            SolicitudDetalleVM mat = new SolicitudDetalleVM();
            //mat.Material = "ACEBUTOLOL";
            //mat.UnidadMedida = "Piezas";
            result.Materiales.Add(mat);

            return result;
        }

        public static SolicitudVM SolicitudSinEnviar()
        {
            SolicitudVM result = new SolicitudVM();
            result.Materiales = new List<SolicitudDetalleVM>();
            SolicitudDetalleVM mat = new SolicitudDetalleVM();
            //mat.Material = "ACEBUTOLOL";
            //mat.UnidadMedida = "Piezas";
            mat.CantidadSolicitada = 2;
            mat.idEstatusSolicitud = 1;
            result.Materiales.Add(mat);

            result.Folio = 1;

            result.Descripcion = "Descripcion para atletas del maraton";

            result.DescripcionUso = "ACEBUTOLOL para .....";
            result.Solicitante = new Usuario() { id = "1", NombreCompleto = "Juan" };
            //sol.Autorizador = new Usuario() { idUsuario = "0", Nombre = "" };
            result.cEstatusSolicitud = new EstatusSolicitudVM() { idEstatusSolicitud = 1, EstatusSolicitud = "Sin Enviar" };

            return result;
        }

        public static SolicitudVM SolicitudAutorizar()
        {
            SolicitudVM result = new SolicitudVM();
            result.Materiales = new List<SolicitudDetalleVM>();
            SolicitudDetalleVM mat = new SolicitudDetalleVM();
            //mat.Material = "ACEBUTOLOL";
            //mat.UnidadMedida = "Piezas";
            mat.CantidadSolicitada = 2;
            mat.idEstatusSolicitud = 2;
            result.Materiales.Add(mat);

            result.Folio = 1;

            result.Descripcion = "Descripcion para atletas del maraton";

            result.DescripcionUso = "ACEBUTOLOL para .....";
            result.Solicitante = new Usuario() { id = "1", NombreCompleto = "Juan" };
            //sol.Autorizador = new Usuario() { idUsuario = "0", Nombre = "" };
            result.cEstatusSolicitud = new EstatusSolicitudVM() { idEstatusSolicitud = 2, EstatusSolicitud = "Por Autorizar" };


            return result;
        }

        public static SolicitudVM SolicitudPendiente()
        {
            SolicitudVM result = new SolicitudVM();
            result.Materiales = new List<SolicitudDetalleVM>();
            SolicitudDetalleVM mat = new SolicitudDetalleVM();
            //mat.Material = "ACEBUTOLOL";
            //mat.UnidadMedida = "Piezas";
            mat.CantidadSolicitada = 2;
            mat.CantidadAutorizada = 2;
            mat.idEstatusSolicitud = 3;
            result.Materiales.Add(mat);

            result.Folio = 1;

            result.Descripcion = "Descripcion para atletas del maraton";

            result.DescripcionUso = "ACEBUTOLOL para .....";
            result.Solicitante = new Usuario() { id = "1", NombreCompleto = "Juan" };
            result.Autorizador = new Usuario() { id = "1", NombreCompleto = "Pedro" };
            result.cEstatusSolicitud = new EstatusSolicitudVM() { idEstatusSolicitud = 3, EstatusSolicitud = "Pendiente" };

            return result;
        }

        public static SolicitudVM SolicitudSurtida()
        {
            SolicitudVM result = new SolicitudVM();
            result.Materiales = new List<SolicitudDetalleVM>();
            SolicitudDetalleVM mat = new SolicitudDetalleVM();
            //mat.Material = "ACEBUTOLOL";
            //mat.UnidadMedida = "Piezas";
            mat.CantidadSolicitada = 2;
            mat.CantidadAutorizada = 2;
            mat.CantidadSurtida = 2;
            mat.CantidadDevolver = 1;
            mat.idEstatusSolicitud = 4;
            result.Materiales.Add(mat);

            result.Folio = 1;

            result.Descripcion = "Descripcion para atletas del maraton";

            result.DescripcionUso = "ACEBUTOLOL para .....";
            result.Solicitante = new Usuario() { id = "1", NombreCompleto = "Ana" };
            result.Autorizador = new Usuario() { id = "1", NombreCompleto = "Pedro" };
            result.cEstatusSolicitud = new EstatusSolicitudVM() { idEstatusSolicitud = 3, EstatusSolicitud = "Surtida" };

            return result;
        }

        public static SolicitudVM Devolucion()
        {
            SolicitudVM result = new SolicitudVM();
            result.Materiales = new List<SolicitudDetalleVM>();
            SolicitudDetalleVM mat = new SolicitudDetalleVM();
            //mat.Material = "ACEBUTOLOL";
            //mat.UnidadMedida = "Piezas";
            mat.CantidadSolicitada = 2;
            mat.CantidadAutorizada = 2;
            mat.idEstatusSolicitud = 0;
            result.Materiales.Add(mat);

            result.Folio = 1;

            result.Descripcion = "Descripcion para atletas del maraton";

            result.DescripcionUso = "ACEBUTOLOL para .....";
            result.Solicitante = new Usuario() { id = "1", NombreCompleto = "Juan" };
            result.Autorizador = new Usuario() { id = "1", NombreCompleto = "Pedro" };
            result.cEstatusSolicitud = new EstatusSolicitudVM() { idEstatusSolicitud = 0, EstatusSolicitud = "Surtida" };

            return result;
        }

        public static List<SolicitudVM> Solicitudes()
        {
            List<SolicitudVM> result = new List<SolicitudVM>();

            SolicitudVM sol = SolicitudSinEnviar();

            result.Add(sol);

            return result;
        }

        public static List<SolicitudVM> SolicitudesAutorizar()
        {
            List<SolicitudVM> result = new List<SolicitudVM>();

            SolicitudVM sol = SolicitudAutorizar();

            result.Add(sol);
            return result;
        }

        public static List<SolicitudVM> SolicitudesPendientes()
        {
            List<SolicitudVM> result = new List<SolicitudVM>();

            SolicitudVM sol = SolicitudPendiente();

            result.Add(sol);

            return result;
        }
        public static List<SolicitudVM> SolicitudesSurtidas()
        {
            List<SolicitudVM> result = new List<SolicitudVM>();

            SolicitudVM sol = SolicitudSurtida();

            result.Add(sol);

            return result;
        }

        public static Alerta AlertaCreate()
        {
            Alerta result = new Alerta();
            result.idAlerta = 0;
            result.Nombre = "";
            result.Valor = "";

            return result;
        }

        public static Alerta AlertaCaducos()
        {
            Alerta result = new Alerta();
            result.idAlerta = 1;
            result.Nombre = "Caducos";
            result.Valor = "1 mes";

            return result;
        }
        public static Alerta AlertaSinExistencia()
        {
            Alerta result = new Alerta();
            result.idAlerta = 2;
            result.Nombre = "Sin Existencia";
            result.Valor = "5 %";

            return result;
        }

        public static List<Alerta> Alertas(int? id=null)
        {
            List<Alerta> result = new List<Alerta>();

            result.Add(AlertaCaducos());
            result.Add(AlertaSinExistencia());

            if(id.HasValue)
            {
                result = result.Where(w => w.idAlerta == id).ToList();
            }

            return result;
        }

        public static AlertaDestinatarioVM AlertaDestinatarioEdit()
        {

            AlertaDestinatarioVM result = new AlertaDestinatarioVM();
            Alerta a = new Alerta();
            a.idAlerta = 1;
            a.Nombre = "Caducos";
            a.Valor = "1 mes";

            result.tAlerta = a;
            result.Rol.Name = "Administrador";
            
            return result;
        }

        public static List<AlertaDestinatarioVM> AlertasDestinatarios()
        {
            List<AlertaDestinatarioVM> result = new List<AlertaDestinatarioVM>();

            AlertaDestinatarioVM r = AlertaDestinatarioEdit();

            result.Add(r);

            return result;
        }

    }
}