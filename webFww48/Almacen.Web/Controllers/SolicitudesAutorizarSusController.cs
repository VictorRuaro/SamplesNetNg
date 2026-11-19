using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacen.Web.Logica;
using Almacen.Web.ViewModels;
using Almacen.Web.Aplicacion;
using Almacen.Web.Applicacion;
using Almacen.Data;
using Almacen.Web.ViewModels.Catalogos;
using Almacen.Infraestructura;
using Almacen.Web.Tools;
using System.Web.Routing;
using RedWillow.MvcToastrFlash;
using Microsoft.AspNet.Identity;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador + "," + EnumRol.Administrador_Informacion + "," + EnumRol.Autorizador_Sustancias)]
    public class SolicitudesAutorizarSusController : Controller
    {
        private SolicitudSustanciasServicio service = null;
        public SolicitudesAutorizarSusController()
        {
            service = new SolicitudSustanciasServicio(new CndAlmacenDopajeEntities());
        }

        public SolicitudesAutorizarSusController(SolicitudSustanciasServicio catalogService)
        {
            this.service = catalogService;
        }

        protected int idCategoria
        {
            get
            {
                return (int)EnumCategoria.Sustancias;
            }
        }
        // GET: SolicitudesAutorizar
        public ActionResult Index(SolicitudFiltrosVM Filtros = null, int pagina = 1)
        {

            BaseListaCatalogosVM<SolicitudVM> model = new BaseListaCatalogosVM<SolicitudVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;



            var parametros = new ParametrosQuery<tSolicitud>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

            var query = PredicateBuilder.True<tSolicitud>();

            System.Linq.Expressions.ExpressionType cNodeType = query.Body.NodeType;
            if (Filtros != null)
            {
                if (Filtros.Folio.HasValue)
                {
                    query = query.And(w => w.Folio == Filtros.Folio);
                }

                if (!string.IsNullOrEmpty(Filtros.Descripcion))
                {
                    query = query.And(w => w.Descripcion.Contains(Filtros.Descripcion));
                }

                if (!string.IsNullOrEmpty(Filtros.DescripcionUso))
                {
                    query = query.And(w => w.DescripcionUso.Contains(Filtros.DescripcionUso));
                }

                if (!string.IsNullOrEmpty(Filtros.idSolicitante))
                {
                    query = query.And(w => w.idSolicitante == Filtros.idSolicitante);
                }


                if (Filtros.EstatusAutorizadas)
                {

                    if (User.IsInRole(EnumRol.Administrador))
                    {
                        query = query.And(w => w.idAutorizador != null && w.cEstatusSolicitud.NumEstatus >= (int)EnumEstatus.PorAutorizar);
                    }
                    else
                    {
                        Filtros.idAutorizador = User.Identity.GetUserId();
                        query = query.And(w => w.idAutorizador == Filtros.idAutorizador && w.cEstatusSolicitud.NumEstatus >= (int)EnumEstatus.PorAutorizar);
                    }

                }
                else
                {
                    query = query.And(w => w.cEstatusSolicitud.NumEstatus == (int)EnumEstatus.PorAutorizar);
                }

            }

            if (query.Body.NodeType != cNodeType)
            {
                parametros.Where = query;
            }

            
            parametros.Where = query.And(w => w.idCategoria == this.idCategoria);

            model = service.ObtenerPaginado(parametros);

            if (query.Body.NodeType == cNodeType && model.Paginacion.TotalRegistros == 0) // si no hay filtros de busqueda y no hay elementos en la lista 
            {
                model.SinDatosListado = true;
            }

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Folio;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["DescripcionUso"] = Filtros.DescripcionUso;
            model.Paginador.ValoresQueryString["idEstatusSolicitud"] = Filtros.idEstatusSolicitud;

            model.Paginador.ValoresQueryString["idSolicitante"] = Filtros.idSolicitante;
            model.Paginador.ValoresQueryString["idAutorizador"] = Filtros.idAutorizador;

            ViewBag.SolicitudFiltros = Filtros;


            //Datos para Filtros

            //List<EstatusSolicitudVM> ListaEstatusSolicitud = new List<EstatusSolicitudVM>();
            List<AspNetUsers> ListaSolicitante = new List<AspNetUsers>();
            //List<AspNetUsers> ListaAutorizador = new List<AspNetUsers>();

            //List<int?> listestatus = new List<int?>() {  (int)EnumEstatus.PorAutorizar, (int)EnumEstatus.Pendiente } ;

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                //ListaEstatusSolicitud = new LeerCatalogosServicio<EstatusSolicitudVM, cEstatusSolicitud>(db).ObtenerTodos(w => w.NumEstatus>= (int)EnumEstatus.PorAutorizar);
                ListaSolicitante = new RepositoryGenerico<AspNetUsers>(db).ObtenerTodos(w => w.AspNetRoles.Any(wr => wr.Name == EnumRol.Solicitante_Sustancias || wr.Name == EnumRol.Administrador));
                //ListaAutorizador = new RepositoryGenerico<AspNetUsers>(db).ObtenerTodos(w => w.AspNetRoles.Any(wr => wr.Name == EnumRol.Autorizador_Sustancias || wr.Name == EnumRol.Administrador));

            }

            //ViewBag.idEstatusSolicitud = CustomExtension.convertSelectList(ListaEstatusSolicitud, "idEstatusSolicitud", "EstatusSolicitud", true, Filtros.idEstatusSolicitud);
            ViewBag.idSolicitante = CustomExtension.convertSelectList(ListaSolicitante, "id", "NombreCompleto", true, Filtros.idSolicitante);
            //ViewBag.idAutorizador = CustomExtension.convertSelectList(ListaAutorizador, "id", "NombreCompleto", false, Filtros.idAutorizador);

            ViewBag.ModuloAutorizar = true;

            return View(model);
        }

        // GET: SolicitudesAutorizar/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            var model = service.ObtenerPorId(id);
            if (model == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            ViewBag.idEstatusSolicitud = model.cEstatusSolicitud.NumEstatus;
            ViewBag.ModuloAutorizar = true;

            model.ValidaFlujo = LogicaAplicacion.ValidaSolicitudFlujo(model);

            return View(model);
        }

        // GET: SolicitudesAutorizar/Edit/5
        public ActionResult Autorizar(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            var model = service.ObtenerPorId(id);
            if (model == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            ViewBag.idEstatusSolicitud = model.cEstatusSolicitud.NumEstatus;

            model.ValidaFlujo = LogicaAplicacion.ValidaSolicitudFlujo(model);

            return View(model);
        }

        // POST: SolicitudMateriales/Autorizar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Autorizar(SolicitudVM model, string btnAccion)
        {

            string mensaje = "";
            bool SinAutorizar = false;
            if (model.idEstatusSolicitud == (int)EnumEstatus.PorAutorizar)
            {
                SinAutorizar = true;
            }

            if (model.Materiales != null)
            {
                //se limpian os errores generados por el rangevalidator cuando CantidadAutorizada y CantidadSurtida estan en cero
                foreach (var item in model.Materiales)
                {
                    if (SinAutorizar)
                    {
                        ModelState["Materiales[" + item.index + "].CantidadSolicitada"].Errors.Clear();
                        ModelState["Materiales[" + item.index + "].CantidadSurtida"].Errors.Clear();
                        //ModelState["Materiales[" + item.index + "].idAlmacen"].Errors.Clear();
                        //ModelState["Materiales[" + item.index + "].CantidadDevolver"].Errors.Clear();
                        //ModelState["Materiales[" + item.index + "].CantidadDevuelta"].Errors.Clear();
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {

                    bool Autorizar = false;
                    if (btnAccion == "Autorizar")
                    {
                        Autorizar = true;
                    }

                    service.Editar(model, model.idSolicitud, Autorizar);

                    var result = service.Save();
                    mensaje = result.Mensaje;
                    if (result.Afectados > 0)
                    {
                        //Envia correo de solicitud Autorizada
                        if (Autorizar)
                        {
                            Usuario usr = new Usuario();
                            using (LeerCatalogosServicio<Usuario, AspNetUsers> db = new LeerCatalogosServicio<Usuario, AspNetUsers>())
                            {
                                usr = db.ObtenerPorId(model.UG);
                            }
                            CorreoServicio correo = new CorreoServicio();
                            ResultCorreo enviado=correo.EnviaCorreo(
                                new DatosCorreo()
                                {
                                    recipients = usr.Email,
                                    copy_recipients = "",
                                    subject="Autorización de solicitud",
                                    body = string.Format("Se informa que ha sido autorizada la solicitud con el folio {0} de Almacen de Sustancias",model.Folio),
                                    body_format="HTML"
                                }
                                );
                            if (!enviado.correoEnviado)
                            {
                                this.Flash(Toastr.WARNING, "Correo", enviado.mensajeError);
                            }
                        }

                        this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mensaje))
                        {
                            this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
                        }
                    }
                    if (Autorizar)
                    {
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Details", new { id = model.idSolicitud });

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

                }
            }

            ViewBag.idEstatusSolicitud = model.cEstatusSolicitud.NumEstatus;

            model.ValidaFlujo = LogicaAplicacion.ValidaSolicitudFlujo(model);


            return View(model);
        }

        //[ValidateAntiForgeryToken]
        //public ActionResult AccionAutorizar(int idSolicitud, int idEstatusSolicitud)
        //{


        //    string mensaje = "";
        //    try
        //    {
        //        // TODO: Add insert logic here
        //        service.Autorizar(idSolicitud);
        //        var result = service.Save();
        //        mensaje = result.Mensaje;
        //        if (result.Afectados > 0)
        //        {
        //            this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(mensaje))
        //            {
        //                this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
        //            }
        //        }
        //        return RedirectToAction("Index");

        //    }
        //    catch (Exception ex)
        //    {
        //        mensaje = ex.Message;
        //        this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

        //    }

        //    //se envia a la pangina de listado de solicitudes
        //    return RedirectToAction("Index");

        //}

        //[ValidateAntiForgeryToken]
        //public ActionResult RegresarSinEnviar(int idSolicitud, int idEstatusSolicitud)
        //{

        //    string mensaje = "";
        //    try
        //    {
        //        // TODO: Add insert logic here
        //        service.CambiarEstatus(idSolicitud, EnumEstatus.SinEnviar);
        //        var result = service.Save();
        //        mensaje = result.Mensaje;
        //        if (result.Afectados > 0)
        //        {
        //            this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(mensaje))
        //            {
        //                this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
        //            }
        //        }
        //        return RedirectToAction("Details", new { id = idSolicitud });

        //    }
        //    catch (Exception ex)
        //    {
        //        mensaje = ex.Message;
        //        this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

        //    }


        //    //se envia a la pangina de listado de solicitudes
        //    return RedirectToAction("Index");

        //}

        // GET: SolicitudesAutorizar/Delete/5
        [ValidateAntiForgeryToken]
        public ActionResult RegresarSinEnviar(int idSolicitud, int idEstatusSolicitud, string UG, string Folio)
        {

            string mensaje = "";
            try
            {
                // TODO: Add insert logic here
                service.CambiarEstatus(idSolicitud, EnumEstatus.SinEnviar);
                var result = service.Save();
                mensaje = result.Mensaje;
                if (result.Afectados > 0)
                {
                    //Envia correo de solicitud Rechazada

                    Usuario usr = new Usuario();
                    using (LeerCatalogosServicio<Usuario, AspNetUsers> db = new LeerCatalogosServicio<Usuario, AspNetUsers>())
                    {
                        usr = db.ObtenerPorId(UG);
                    }
                    CorreoServicio correo = new CorreoServicio();
                    ResultCorreo enviado =correo.EnviaCorreo(
                        new DatosCorreo()
                        {
                            recipients = usr.Email,
                            copy_recipients = "",
                            subject = "Rechazo de solicitud",
                            body = string.Format("Se informa que ha sido rechazada la solicitud con el folio {0} de Almacen de Sustancias", Folio),
                            body_format = "HTML"
                        }
                        );
                    if (!enviado.correoEnviado)
                    {
                        this.Flash(Toastr.WARNING, "Correo", enviado.mensajeError);
                    }
                    this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
                }
                else
                {
                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
                    }
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

            }


            //se envia a la pangina de listado de solicitudes
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SolicitudesAutorizar/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ListaEliminarElemento(int index, List<SolicitudDetalleVM> sustancias, int idEstatusSolicitud)
        {

            List<SolicitudDetalleVM> model = new List<SolicitudDetalleVM>();

            model = sustancias.Where(w => w.index != index).ToList();

            ViewBag.idEstatusSolicitud = idEstatusSolicitud;

            return PartialView("_EditarSolicitudSustancias", model);

        }
        public ActionResult ListaMateriales(int idSolicitud, int idEstatusSolicitud)
        {
            List<SolicitudDetalleVM> model = new List<SolicitudDetalleVM>();

            var datosbd = service.ObtenerPorId(idSolicitud);
            model.AddRange(datosbd.Materiales);

            ViewBag.idEstatusSolicitud = idEstatusSolicitud;

            return PartialView("_EditarSolicitudSustancias", model);
        }
    }
}
