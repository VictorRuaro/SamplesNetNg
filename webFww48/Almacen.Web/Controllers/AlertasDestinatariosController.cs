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

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador)]
    public class AlertasDestinatariosController : Controller
    {
        private ICatalogosServicio<AlertaDestinatarioVM, tAlertaDestinatario> service = null;
        public AlertasDestinatariosController()
        {
            service = new CatalogosServicio<AlertaDestinatarioVM, tAlertaDestinatario>(new CndAlmacenDopajeEntities());
        }

        public AlertasDestinatariosController(ICatalogosServicio<AlertaDestinatarioVM, tAlertaDestinatario> catalogService)
        {
            this.service = catalogService;
        }

        // GET: CatConservacion
        public ActionResult Index(string busqueda, int pagina = 1)
        {

            BaseListaCatalogosVM<AlertaDestinatarioVM> model = new BaseListaCatalogosVM<AlertaDestinatarioVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;

            var parametros = new ParametrosQuery<tAlertaDestinatario>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);
            List<AspNetRoles> ListaRoles = new List<AspNetRoles>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaRoles = new RepositoryGenerico<AspNetRoles>(db).ObtenerTodos();
            }

            if (!string.IsNullOrEmpty(busqueda))
            {

                List<string> ListaId = new List<string>();
                ListaId = ListaRoles.Where(w => w.Name.Contains(busqueda)).Select(s => s.Id).ToList();
                if (ListaRoles.Any())
                {
                    parametros.Where = w => w.tAlerta.Nombre.Contains(busqueda) || ListaId.Contains(w.idRol);
                }
                else
                {
                    parametros.Where = w => w.tAlerta.Nombre.Contains(busqueda);
                }

            }

            model = service.ObtenerPaginado(parametros);
            foreach (var item in model.Paginacion.Lista)
            {
                var rol = ListaRoles.Where(w => w.Id == item.idRol).FirstOrDefault();
                if (rol!=null)
                {
                    item.Rol.Id = rol.Id;
                    item.Rol.Name = rol.Name;
                }
            }
            

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["busqueda"] = busqueda;

            return View(model);
        }

        // GET: CatConservacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            var model = service.ObtenerPorId(id);

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                var rol = new RepositoryGenerico<AspNetRoles>(db).ObtenerPorId(model.idRol);
                if (rol != null)
                {
                    model.Rol.Id = rol.Id;
                    model.Rol.Name = rol.Name;
                }
            }

            if (model == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }
            return View(model);
        }

        // GET: CatConservacion/Create
        public ActionResult Create()
        {
            List<Alerta> ListaAlerta = new List<Alerta>();
            List<AspNetRoles> ListaRoles = new List<AspNetRoles>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaAlerta = new LeerCatalogosServicio<Alerta, tAlerta>(db).ObtenerTodos();
                ListaRoles = new RepositoryGenerico<AspNetRoles>(db).ObtenerTodos();
            }

            ViewBag.idAlerta = CustomExtension.convertSelectList(ListaAlerta, "idAlerta", "Nombre", true);
            ViewBag.idRol = CustomExtension.convertSelectList(ListaRoles, "id", "Name", true);
            return View();
        }

        // POST: CatConservacion/Create
        [HttpPost]
        public ActionResult Create(AlertaDestinatarioVM model)
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
            List<Alerta> ListaAlerta = new List<Alerta>();
            List<AspNetRoles> ListaRoles = new List<AspNetRoles>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaAlerta = new LeerCatalogosServicio<Alerta, tAlerta>(db).ObtenerTodos();
                ListaRoles = new RepositoryGenerico<AspNetRoles>(db).ObtenerTodos();
            }
            ViewBag.idAlerta = CustomExtension.convertSelectList(ListaAlerta, "idAlerta", "Nombre", true);
            ViewBag.idRol = CustomExtension.convertSelectList(ListaRoles, "id", "Name", true);
            return View(model);
        }

        // GET: CatConservacion/Edit/5
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
            List<Alerta> ListaAlerta = new List<Alerta>();
            List<AspNetRoles> ListaRoles = new List<AspNetRoles>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaAlerta = new LeerCatalogosServicio<Alerta, tAlerta>(db).ObtenerTodos();
                ListaRoles = new RepositoryGenerico<AspNetRoles>(db).ObtenerTodos();
            }
            ViewBag.idAlerta = CustomExtension.convertSelectList(ListaAlerta, "idAlerta", "Nombre", true);
            ViewBag.idRol = CustomExtension.convertSelectList(ListaRoles, "id", "Name", true);
            return View(model);
        }

        // POST: CatConservacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, AlertaDestinatarioVM model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idAlertaDestinatario);
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
            List<Alerta> ListaAlerta = new List<Alerta>();
            List<AspNetRoles> ListaRoles = new List<AspNetRoles>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaAlerta = new LeerCatalogosServicio<Alerta, tAlerta>(db).ObtenerTodos();
                ListaRoles = new RepositoryGenerico<AspNetRoles>(db).ObtenerTodos();
            }
            ViewBag.idAlerta = CustomExtension.convertSelectList(ListaAlerta, "idAlerta", "Nombre", true);
            ViewBag.idRol = CustomExtension.convertSelectList(ListaRoles, "id", "Name", true);
            return View(model);
        }

        // GET: CatConservacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            var model = service.ObtenerPorId(id);
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                var rol = new RepositoryGenerico<AspNetRoles>(db).ObtenerPorId(model.idRol);
                if (rol != null)
                {
                    model.Rol.Id = rol.Id;
                    model.Rol.Name = rol.Name;
                }

            }
            if (model == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }
            return View(model);
        }

        // POST: CatConservacion/Delete/5
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
