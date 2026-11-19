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
    public class CategoriaController : Controller
    {
        private ICatalogosServicio<CategoriaVM, cCategoria> service = null;
        public CategoriaController()
        {
            service = new CatalogosServicio<CategoriaVM, cCategoria>(new CndAlmacenDopajeEntities());
        }

        public CategoriaController(ICatalogosServicio<CategoriaVM, cCategoria> catalogService)
        {
            this.service = catalogService;
        }

        // GET: CatCategoria
        public ActionResult Index(string busqueda, int pagina = 1)
        {

            BaseListaCatalogosVM<CategoriaVM> model = new BaseListaCatalogosVM<CategoriaVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;


            var parametros = new ParametrosQuery<cCategoria>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

            if (!string.IsNullOrEmpty(busqueda))
            {
                parametros.Where = w => w.Categoria.Contains(busqueda);
            }
            model = service.ObtenerPaginado(parametros);

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["busqueda"] = busqueda;

            return View(model);
        }

        // GET: CatCategoria/Details/5
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

        // GET: CatCategoria/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatCategoria/Create
        [HttpPost]
        public ActionResult Create(CategoriaVM model)
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

        // GET: CatCategoria/Edit/5
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

        // POST: CatCategoria/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CategoriaVM model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idCategoria);
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

        // GET: CatCategoria/Delete/5
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

        // POST: CatCategoria/Delete/5
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