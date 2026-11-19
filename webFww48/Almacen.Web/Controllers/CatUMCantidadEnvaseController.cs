using Almacen.Data;
using Almacen.Infraestructura;
using Almacen.Web.Aplicacion;
using Almacen.Web.Applicacion;
using Almacen.Web.ViewModels;
using Almacen.Web.ViewModels.Catalogos;
using RedWillow.MvcToastrFlash;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador + "," + EnumRol.Administrador_Informacion)]
    public class CatUMCantidadEnvaseController : Controller
    {
        private ICatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase> service = null;
        public CatUMCantidadEnvaseController()
        {
            service = new CatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(new CndAlmacenDopajeEntities());
        }

        public CatUMCantidadEnvaseController(ICatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase> catalogService)
        {
            this.service = catalogService;
        }

        // GET: CatUMCantidadEnvase
        public ActionResult Index(string busqueda, int pagina = 1)
        {

            BaseListaCatalogosVM<UMCantidadEnvaseVM> model = new BaseListaCatalogosVM<UMCantidadEnvaseVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;


            var parametros = new ParametrosQuery<cUMCantidadEnvase>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

            if (!string.IsNullOrEmpty(busqueda))
            {
                parametros.Where = w => w.UMCantidadEnvase.Contains(busqueda);
            }
            model = service.ObtenerPaginado(parametros);

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["busqueda"] = busqueda;

            return View(model);
        }

        // GET: CatUMCantidadEnvase/Details/5
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

        // GET: CatUMCantidadEnvase/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatUMCantidadEnvase/Create
        [HttpPost]
        public ActionResult Create(UMCantidadEnvaseVM model)
        {
            string mensaje = "";
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
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: CatUMCantidadEnvase/Edit/5
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
            return View(model);
        }

        // POST: CatUMCantidadEnvase/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, UMCantidadEnvaseVM model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idUMCantidadEnvase);
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
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: CatUMCantidadEnvase/Delete/5
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
            return View(model);
        }

        // POST: CatUMCantidadEnvase/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                service.Eliminar(id);
                var result = service.Save();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (service != null)
                {
                    service.Dispose();
                    service = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
