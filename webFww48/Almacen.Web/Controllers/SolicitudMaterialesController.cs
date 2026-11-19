using Almacen.Data;
using Almacen.Infraestructura;
using Almacen.Web.Aplicacion;
using Almacen.Web.Applicacion;
using Almacen.Web.Tools;
using Almacen.Web.ViewModels;
using Almacen.Web.ViewModels.Catalogos;
using RedWillow.MvcToastrFlash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;

namespace Almacen.Web.Controllers
{

    public class SolicitudMaterialesController : Controller
    {
        private SolicitudMaterialesServicio service = null;
        public SolicitudMaterialesController()
        {
            service = new SolicitudMaterialesServicio(new CndAlmacenDopajeEntities());
        }

        public SolicitudMaterialesController(SolicitudMaterialesServicio catalogService)
        {
            this.service = catalogService;
        }

        protected int idCategoria
        {
            get
            {
                return (int)EnumCategoria.Materiales;
            }
        }

        // GET: SolicitudMateriales
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
                    query = query.And(w => w.Folio==Filtros.Folio);
                }

                if (!string.IsNullOrEmpty(Filtros.Descripcion))
                {
                    query = query.And(w => w.Descripcion.Contains(Filtros.Descripcion));
                }

                if (!string.IsNullOrEmpty(Filtros.DescripcionUso))
                {
                    query = query.And(w => w.DescripcionUso.Contains(Filtros.DescripcionUso));
                }

                if (Filtros.idEstatusSolicitud.HasValue)
                {
                    query = query.And(w => w.idEstatusSolicitud == Filtros.idEstatusSolicitud);
                }

                if (!string.IsNullOrEmpty(Filtros.idSolicitante))
                {
                    query = query.And(w => w.idSolicitante == Filtros.idSolicitante);
                }
                if (!string.IsNullOrEmpty(Filtros.idAutorizador))
                {
                    query = query.And(w => w.idAutorizador == Filtros.idAutorizador);
                }

            }

            if (query.Body.NodeType != cNodeType)
            {
                parametros.Where = query;
            }

            if (parametros.Where == null)
            {
                parametros.Where = w => w.idCategoria == this.idCategoria;
            }
            else
            {
                parametros.Where = PredicateBuilder.And(w => w.idCategoria == this.idCategoria, parametros.Where);
            }

            //En caso de que el usuario tenga el rol de solicitante o autorizador solo debe ver sus solicitudes
            bool filtrarSolicitante = true;
            if (User.IsInRole(EnumRol.Administrador)
                || User.IsInRole(EnumRol.Administrador_Informacion)
                )
            {
                filtrarSolicitante = false;
            }

            if (filtrarSolicitante)
            {
                string idusuario = User.Identity.GetUserId();
                parametros.Where = PredicateBuilder.And(w => w.idSolicitante == idusuario, parametros.Where);
            }

            //llenado del modelo

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

            List<EstatusSolicitudVM> ListaEstatusSolicitud = new List<EstatusSolicitudVM>();
            List<AspNetUsers> ListaSolicitante = new List<AspNetUsers>();
            List<AspNetUsers> ListaAutorizador = new List<AspNetUsers>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                

                ListaEstatusSolicitud = new LeerCatalogosServicio<EstatusSolicitudVM, cEstatusSolicitud>(db).ObtenerTodos();
                if (!filtrarSolicitante)
                {
                    ListaSolicitante = new RepositoryGenerico<AspNetUsers>(db).ObtenerTodos(w => w.AspNetRoles.Any(wr => wr.Name == EnumRol.Solicitante_Material || wr.Name == EnumRol.Administrador));
                }
                ListaAutorizador = new RepositoryGenerico<AspNetUsers>(db).ObtenerTodos(w => w.AspNetRoles.Any(wr => wr.Name == EnumRol.Autorizador_Material || wr.Name == EnumRol.Administrador));

            }

            ViewBag.idEstatusSolicitud = CustomExtension.convertSelectList(ListaEstatusSolicitud, "idEstatusSolicitud", "EstatusSolicitud", true, Filtros.idEstatusSolicitud);
            if (!filtrarSolicitante)
            {
                ViewBag.idSolicitante = CustomExtension.convertSelectList(ListaSolicitante, "id", "NombreCompleto", true, Filtros.idSolicitante);
            }
            ViewBag.idAutorizador = CustomExtension.convertSelectList(ListaAutorizador, "id", "NombreCompleto", true, Filtros.idAutorizador); 


            return View(model);
        }


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

            model.ValidaFlujo = LogicaAplicacion.ValidaSolicitudFlujo(model);

            return View(model);
        }


        [ValidateAntiForgeryToken]
        public ActionResult AccionesEnviarAutorizar(int idSolicitud, int idEstatusSolicitud)
        {


            string mensaje = "";
            try
            {
                // TODO: Add insert logic here
                service.EnviarAutorizar(idSolicitud);
                var result = service.Save();
                mensaje = result.Mensaje;
                if (result.Afectados > 0)
                {
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

        [ValidateAntiForgeryToken]
        public ActionResult RegresarSinEnviar(int idSolicitud, int idEstatusSolicitud)
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

        // GET: SolicitudMateriales/Create
        public ActionResult Create()
        {
            SolicitudVM model = new SolicitudVM();

            model.idEstatusSolicitud = (int)EnumEstatus.Borrador;
            model.idCategoria = this.idCategoria;

            // lista de Grado en el buscador
            List<GradoVM> ListaGrupoFarmacologico = new List<GradoVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GradoVM,cGrado>(db).ObtenerTodos();
            }

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrado", "Grado", true);


            return View(model);
        }

        // POST: SolicitudMateriales/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitudVM model, string post)
        {
            string mensaje = "";
            model.idCategoria = idCategoria;
            model.idEstatusSolicitud = (int)EnumEstatus.Borrador;
            ViewBag.idEstatusSolicitud = model.idEstatusSolicitud;

            if (model.Materiales!=null)
            {
                //se limpian os errores generados por el rangevalidator cuando CantidadAutorizada y CantidadSurtida estan en cero
                foreach (var item in model.Materiales)
                {
                    if (item.idEstatusSolicitud == (int)EnumEstatus.Borrador)
                    {
                        ModelState["Materiales[" + item.index + "].CantidadAutorizada"].Errors.Clear();
                        ModelState["Materiales[" + item.index + "].CantidadSurtida"].Errors.Clear();
                        //ModelState["Materiales[" + item.index + "].CantidadDevolver"].Errors.Clear();
                        //ModelState["Materiales[" + item.index + "].CantidadDevuelta"].Errors.Clear();
                    }
                }
            }


            if (ModelState.IsValid)
            {
                try
                {
                    model.idEstatusSolicitud = (int)EnumEstatus.SinEnviar;

                    // TODO: Add insert logic here
                    SolicitudVM sol=service.Agregar(model);
                    var result = service.Save();
                    //var result = new ResultSave() { Afectados = 1,Mensaje="simulacion guarda" };
                    mensaje = result.Mensaje;
                    if (result.Afectados > 0)
                    {
                        this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
                        sol = service.Obtener(w => w.Folio == sol.Folio);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mensaje))
                        {
                            this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
                        }
                    }
                    return RedirectToAction("Details",new { id = sol.idSolicitud });

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
                    //ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true, model.idGrado);
                    //return View(model);
                }
            }

            // lista de Grado en el buscador
            List<GradoVM> ListaGrupoFarmacologico = new List<GradoVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();
            }

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrado", "Grado", true);


            return View(model);
        }


        // GET: SolicitudMateriales/Edit/5
        public ActionResult Edit(int? id)
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

            if(model.cEstatusSolicitud.NumEstatus >(int)EnumEstatus.SinEnviar)
            {
                return RedirectToAction("Details", new { id = model.idSolicitud });
            }

            ViewBag.idEstatusSolicitud = model.cEstatusSolicitud.NumEstatus;

            // lista de Grado en el buscador
            List<GradoVM> ListaGrupoFarmacologico = new List<GradoVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();
            }

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrado", "Grado", true);

            return View(model);
        }

        // POST: SolicitudMateriales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SolicitudVM model)
        {
            string mensaje = "";
            bool SinAutorizar=false;
            if (model.cEstatusSolicitud.NumEstatus == (int)EnumEstatus.Borrador || model.cEstatusSolicitud.NumEstatus == (int)EnumEstatus.SinEnviar)
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
                        ModelState["Materiales[" + item.index + "].CantidadAutorizada"].Errors.Clear();
                        ModelState["Materiales[" + item.index + "].CantidadSurtida"].Errors.Clear();
                        //ModelState["Materiales[" + item.index + "].CantidadDevolver"].Errors.Clear();
                        //ModelState["Materiales[" + item.index + "].CantidadDevuelta"].Errors.Clear();
                    }
                }
            }


            if (ModelState.IsValid)
            {
                try
                {

                    // TODO: Add insert logic here
                    service.Editar(model, model.idSolicitud);
                    var result = service.Save();
                    mensaje = result.Mensaje;
                    if (result.Afectados > 0)
                    {
                        this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mensaje))
                        {
                            this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
                        }
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

            // lista de Grado en el buscador
            List<GradoVM> ListaGrupoFarmacologico = new List<GradoVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();
            }

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrado", "Grado", true);

            return View(model);
        }

        // GET: SolicitudMateriales/Delete/5
        public ActionResult Delete(int? id)
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
            return View(model);
        }

        // POST: SolicitudMateriales/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            string mensaje = "";
            try
            {
                // TODO: Add delete logic here
                service.Eliminar(id);
                var result = service.Save();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return View();
            }
        }

        public ActionResult ListadoMaterialesSelect(string Codigo, string Descripcion, string idGrado)
        {

            List<MaterialVM> model = new List<MaterialVM>();

            #region Filtros
            MaterialFiltrosVM Filtros = new MaterialFiltrosVM();

            var parametros = new Parametros<tDetalleMaterial>();

            int nidGrado = 0;

            if (int.TryParse(idGrado, out nidGrado))
            {
                Filtros.idGrado = nidGrado;
            }

            Filtros.idCategoria = this.idCategoria;

            Filtros.Codigo = Codigo;
            Filtros.Descripcion = Descripcion;

            //Filtros.Codigo = FormulaMolecular;

            if (Filtros != null)
            {
                var query = PredicateBuilder.True<tDetalleMaterial>();

                if (!string.IsNullOrEmpty(Filtros.Codigo))
                {
                    query = query.And(w => w.Codigo.Contains(Filtros.Codigo));
                }

                if (!string.IsNullOrEmpty(Filtros.Descripcion))
                {
                    query = query.And(w => w.Descripcion.Contains(Filtros.Descripcion));
                }

                if (Filtros.idGrado > 0)
                {
                    query = query.And(w => w.idGrado == Filtros.idGrado);
                }

                //if (!string.IsNullOrEmpty(Filtros.FormulaMolecular))
                //{
                //    query = query.And(w => w.FormulaMolecular.Contains(Filtros.FormulaMolecular));
                //}

                if (query != PredicateBuilder.True<tDetalleMaterial>())
                {
                    parametros.Where = query;
                }
            } 
            #endregion

            List<GradoVM> ListaGrupoFarmacologico = new List<GradoVM>();
            DateTime fechacaducidad = DateTime.Now;
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                if (parametros.Where != null)
                {
                    parametros.Where = PredicateBuilder.And(w => w.tAlmacen.Any()
                                                            && w.tAlmacen.Where(a => a.ST == true).Any()
                                                            && w.tAlmacen.Where(a => a.FechaCaducidad > fechacaducidad).Any()
                                                            , parametros.Where);
                }
                else
                {
                    parametros.Where = w => w.tAlmacen.Any()
                                           && w.tAlmacen.Where(a => a.ST == true).Any()
                                           && w.tAlmacen.Where(a => a.FechaCaducidad > fechacaducidad).Any();
                }

                ListaGrupoFarmacologico = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();
                model = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerTodos(parametros.Where);
            }

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrado", "Grado", true, Filtros.idGrado);


            return PartialView("_ListadoMaterialesSelect", model);
        }
        [HttpPost]
        public ActionResult ListaSolicitudMateriales(object[] selecionados, List<SolicitudDetalleVM> materiales,int idEstatusSolicitud)
        {
            List<SolicitudDetalleVM> model = new List<SolicitudDetalleVM>();


            List<int> lista = selecionados.Select(s=> int.Parse(s.ToString())).ToList();

            if (materiales != null)
            {
                List<int> listaExistentes = materiales.Select(s => s.idSustanciaMaterial).ToList();
                lista = lista.Except(listaExistentes).ToList();
                model = materiales;
            }

            List<MaterialVM> materialesAgregar = new List<MaterialVM>();
            
            List<SolicitudDetalleVM> datosbd = new List<SolicitudDetalleVM>();
            List<PresentacionMaterialVM> datosAlmacenbd = new List<PresentacionMaterialVM>();

            if (lista.Count > 0)
            {
                using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
                {
                    materialesAgregar = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db)
                        .ObtenerTodos(w => lista.Contains(w.idSustanciaMaterial));

                }

                datosbd = materialesAgregar.Select(s =>
                new SolicitudDetalleVM()
                {
                    idSustanciaMaterial = s.idSustanciaMaterial,
                    Descripcion = s.Descripcion,
                    Material = s,
                    idCategoria=s.idCategoria
                }
                ).ToList();
            }

           

            if (datosbd!=null)
            {
                model.AddRange(datosbd);
            }

            foreach (var item in model)
            {
                var alm = LogicaAplicacion.ObtenerPresentacionesMaterial(item.idSustanciaMaterial);

                if (alm.Count() == 1)
                {
                    item.idAlmacen = alm.FirstOrDefault().idAlmacen;
                    item.idTipoEnvase = alm.FirstOrDefault().idPresentacion;
                    item.idPresentacion = alm.FirstOrDefault().idPresentacion;
                }
                else
                {

                }


            }

            ViewBag.idEstatusSolicitud = idEstatusSolicitud;

            return PartialView("_EditarSolicitudMateriales", model);
        }

        [HttpPost]
        public ActionResult ListaEliminarElemento(int index, List<SolicitudDetalleVM> materiales, int idEstatusSolicitud)
        {

            List<SolicitudDetalleVM> model = new List<SolicitudDetalleVM>();

            model = materiales.Where(w =>w.index != index).ToList();

            ViewBag.idEstatusSolicitud = idEstatusSolicitud;

            return PartialView("_EditarSolicitudMateriales", model);

        }

        // Presentaciones de Materiales
        public ActionResult PresentacionesMaterial(int idSustanciaMaterial)
        {

            List<PresentacionMaterialVM> model = new List<PresentacionMaterialVM>();

            model = model = LogicaAplicacion.ObtenerPresentacionesMaterial(idSustanciaMaterial);


            return PartialView("_PresentacionesMaterial", model);
        }

        [HttpPost]
        public ActionResult ListaPresentacionMateriales(List<PresentacionMaterialVM> selecionados, List<SolicitudDetalleVM> materiales)
        {
            List<SolicitudDetalleVM> model = new List<SolicitudDetalleVM>();


            foreach (var item in materiales)
            {
                var alm = selecionados.Where(w => w.idSustanciaMaterial == item.idSustanciaMaterial);

                if (alm.Count() == 1)
                {
                    item.idAlmacen = alm.FirstOrDefault().idAlmacen;
                    item.idTipoEnvase = alm.FirstOrDefault().idTipoEnvase;
                    item.idPresentacion = alm.FirstOrDefault().idPresentacion;
                }

            }

            model = materiales;

            return PartialView("_EditarSolicitudMateriales", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (serviceGrado != null)
                //{
                //    serviceGrado.Dispose();
                //    serviceGrado = null;
                //}

                //if (service != null)
                //{
                //    service.Dispose();
                //    service = null;
                //}
            }

            base.Dispose(disposing);
        }
    }
}
