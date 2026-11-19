using Almacen.Web.Applicacion;

using Almacen.Web.ViewModels.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacen.Data;
using RedWillow.MvcToastrFlash;
using Almacen.Infraestructura;
using AutoMapper;
using Almacen.Web.ViewModels;
using System.Web.Routing;
using Almacen.Web.Aplicacion;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador + "," + EnumRol.Administrador_Informacion)]
    public class CatGrupoFarmacologicoController : Controller
    {
        private ICatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico> service = null;

        public CatGrupoFarmacologicoController()
        {
            service = new CatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico>(new CndAlmacenDopajeEntities());
        }

        public CatGrupoFarmacologicoController(ICatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico> catalogService)
        {
            this.service = catalogService;
        }

        // GET: CatGrupoFarmacologico
        public ActionResult Index(string busqueda, int pagina=1)
        {

            BaseListaCatalogosVM<GrupoFarmacologicoVM> model = new BaseListaCatalogosVM<GrupoFarmacologicoVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;
            

            var parametros = new ParametrosQuery<cGrupoFarmacologico>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

            if(!string.IsNullOrEmpty(busqueda))
            {
                parametros.Where = w => w.GrupoFarmacologico.Contains(busqueda);
            }
            model = service.ObtenerPaginado(parametros);

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["busqueda"] = busqueda;

            return View(model);
        }


        //[HttpPost]
        //public ActionResult Index(GrupoFarmacologicoVM model,int pagina=1)
        //{
        //    var parametros = new ParametrosQuery<cGrupoFarmacologico>(model.Paginacion.PaginaActual, model.Paginacion.RegistrosPorPagina);

        //    parametros.Where = x => x.descripcion == model.Filtros.descripcion;

        //    model.Paginacion = service.ObtenerPaginado(parametros);

        //    model.Paginador = Mapper.Map<Paginacion<GrupoFarmacologico>, PaginadorViewModels>(model.Paginacion);

        //    return View(model);
        //}


        // GET: CatGrupoFarmacologico/Details/5
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

        // GET: CatGrupoFarmacologico/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatGrupoFarmacologico/Create
        [HttpPost]
        public ActionResult Create(GrupoFarmacologicoVM model)
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
                    if (result.Afectados>0)
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

        // GET: CatGrupoFarmacologico/Edit/5
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

        // POST: CatGrupoFarmacologico/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, GrupoFarmacologicoVM model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idGrupoFarmacologico);
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

        // GET: CatGrupoFarmacologico/Delete/5
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

        // POST: CatGrupoFarmacologico/Delete/5
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
