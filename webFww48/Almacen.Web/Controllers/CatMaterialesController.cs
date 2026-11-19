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
using System.Web.Mvc;
using System.Web.Routing;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador + "," + EnumRol.Administrador_Informacion)]
    public class CatMaterialesController : Controller
    {
        private ICatalogosServicio<MaterialVM, tDetalleMaterial> service = null;
        private ILeerCatalogosServicio<GradoVM, cGrado> serviceGrado = null;
        public CatMaterialesController()
        {
            service = new CatalogosServicio<MaterialVM, tDetalleMaterial>(new CndAlmacenDopajeEntities());
            serviceGrado = new LeerCatalogosServicio<GradoVM, cGrado>(new CndAlmacenDopajeEntities());
        }

        public CatMaterialesController(ICatalogosServicio<MaterialVM, tDetalleMaterial> catalogService, ILeerCatalogosServicio<GradoVM, cGrado> gradoservice)
        {
            this.service = catalogService;
            this.serviceGrado = gradoservice;
        }

        protected int idCategoria
        {
            get
            {
                return (int)EnumCategoria.Materiales;
            }
        }

        // GET: CatMaterial
        public ActionResult Index(MaterialFiltrosVM Filtros = null, int pagina = 1)
        {

            BaseListaCatalogosVM<MaterialVM> model = new BaseListaCatalogosVM<MaterialVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;

            ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true, Filtros.idGrado);

            var parametros = new ParametrosQuery<tDetalleMaterial>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);


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


                if (query != PredicateBuilder.True<tDetalleMaterial>())
                {
                    parametros.Where = query;
                }
            }

            model = service.ObtenerPaginado(parametros);

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Codigo"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["idGrado"] = Filtros.idGrado;

            ViewBag.MaterialFiltros = Filtros;

            return View(model);
        }

        // GET: CatMaterial/Details/5
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

            return View(model);
        }

        // GET: CatMaterial/Create
        public ActionResult Create()
        {
            ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true);
            return View();
        }

        // POST: CatMaterial/Create
        [HttpPost]
        public ActionResult Create(MaterialVM model)
        {
            string mensaje = "";
            model.idCategoria = idCategoria;
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Agregar(model);
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
                    ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true, model.idGrado);
                    return View(model);
                }
            }
            ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true, model.idGrado);
            return View(model);
        }

        // GET: CatMaterial/Edit/5
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
            ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true);
            return View(model);
        }

        // POST: CatMaterial/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, MaterialVM model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idSustanciaMaterial);
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
                    ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true);
                    return View(model);
                }
            }
            ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true, model.idGrado);
            return View(model);
        }

        // GET: CatMaterial/Delete/5
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

            int existencia = 0;

            var lista = LogicaAplicacion.ListaExistencias(this.idCategoria, new List<int>() { id.Value });

            existencia = LogicaAplicacion.ExistenciasPorFechaLote(lista, id.Value, true);

            if (existencia == 0)
            {
                ViewBag.visiblebtnEliminar = true;
            }

            return View(model);
        }

        // POST: CatMaterial/Delete/5
        [HttpPost]
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
