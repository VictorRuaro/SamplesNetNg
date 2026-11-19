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
    public class CatSustanciasController : Controller
    {
        private ICatalogosServicio<SustanciaVM, tDetalleSustacia> service = null;
        private ILeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico> serviceGrupoFarmacologico = null;
        public CatSustanciasController()
        {
            service = new CatalogosServicio<SustanciaVM, tDetalleSustacia>(new CndAlmacenDopajeEntities());
            serviceGrupoFarmacologico = new LeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico>(new CndAlmacenDopajeEntities());
        }

        public CatSustanciasController(ICatalogosServicio<SustanciaVM, tDetalleSustacia> catalogService, ILeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico> grupoFarmacologicoservice)
        {
            this.service = catalogService;
            this.serviceGrupoFarmacologico = grupoFarmacologicoservice;
        }

        protected int idCategoria
        {
            get
            {
                return (int)EnumCategoria.Sustancias;
            }
        }

        // GET: CatSustancia
        public ActionResult Index(SustanciaFiltrosVM Filtros =null, int pagina = 1)
        {

            BaseListaCatalogosVM<SustanciaVM> model = new BaseListaCatalogosVM<SustanciaVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;
            
            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(serviceGrupoFarmacologico.ObtenerTodos(), "idGrupoFarmacologico", "GrupoFarmacologico",true,Filtros.idGrupoFarmacologico);

            var parametros = new ParametrosQuery<tDetalleSustacia>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

            

            if (Filtros != null)
            {
                var query = PredicateBuilder.True<tDetalleSustacia>();
                System.Linq.Expressions.ExpressionType  cNodeType = query.Body.NodeType;

                if (!string.IsNullOrEmpty(Filtros.Codigo))
                {
                    query = query.And(w => w.Codigo.Contains(Filtros.Codigo));
                }

                if (!string.IsNullOrEmpty(Filtros.Descripcion))
                {
                    query = query.And(w => w.Descripcion.Contains(Filtros.Descripcion));
                }

                if (Filtros.idGrupoFarmacologico > 0)
                {
                    query = query.And(w => w.idGrupoFarmacologico == Filtros.idGrupoFarmacologico);
                }

                if (!string.IsNullOrEmpty(Filtros.Sinonimos))
                {
                    query = query.And(w => w.CAS.Contains(Filtros.Sinonimos));
                }

                if (!string.IsNullOrEmpty(Filtros.CAS))
                {
                    query = query.And(w => w.CAS.Contains(Filtros.CAS));
                }

                if (!string.IsNullOrEmpty(Filtros.PMCompuesto))
                {
                    query = query.And(w => w.PMCompuesto.Contains(Filtros.PMCompuesto));
                }

                if (!string.IsNullOrEmpty(Filtros.FormulaMolecular))
                {
                    query = query.And(w => w.FormulaMolecular.Contains(Filtros.FormulaMolecular));
                }

                if (Filtros.EsSustanciaControlada !=null)
                {
                    query = query.And(w => w.EsSustanciaControlada == Filtros.EsSustanciaControlada);
                }

                if (query.Body.NodeType != cNodeType)
                {
                    parametros.Where = query;
                }
            }

            model = service.ObtenerPaginado(parametros);

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Codigo1"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["idGrupoFarmacologico"] = Filtros.idGrupoFarmacologico;
            model.Paginador.ValoresQueryString["Sinonimos"] = Filtros.Sinonimos;
            model.Paginador.ValoresQueryString["CAS"] = Filtros.CAS;
            model.Paginador.ValoresQueryString["PMCompuesto"] = Filtros.PMCompuesto;
            model.Paginador.ValoresQueryString["FormulaMolecular"] = Filtros.FormulaMolecular;
            model.Paginador.ValoresQueryString["EsSustanciaControlada"] = Filtros.EsSustanciaControlada;

            ViewBag.SustanciaFiltros = Filtros;

            return View(model);
        }

        // GET: CatSustancia/Details/5
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

        // GET: CatSustancia/Create
        public ActionResult Create()
        {
            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(serviceGrupoFarmacologico.ObtenerTodos(), "idGrupoFarmacologico", "GrupoFarmacologico", true);
            return View();
        }

        // POST: CatSustancia/Create
        [HttpPost]
        public ActionResult Create(SustanciaVM model)
        {
            string mensaje = "";
            model.idCategoria=idCategoria;
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
                    ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(serviceGrupoFarmacologico.ObtenerTodos(), "idGrupoFarmacologico", "GrupoFarmacologico", true, model.idGrupoFarmacologico);
                    return View(model);
                }
            }
            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(serviceGrupoFarmacologico.ObtenerTodos(), "idGrupoFarmacologico", "GrupoFarmacologico", true, model.idGrupoFarmacologico);
            return View(model);
        }

        // GET: CatSustancia/Edit/5
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
            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(serviceGrupoFarmacologico.ObtenerTodos(), "idGrupoFarmacologico", "GrupoFarmacologico", true);
            return View(model);
        }

        // POST: CatSustancia/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, SustanciaVM model)
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
                    ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(serviceGrupoFarmacologico.ObtenerTodos(), "idGrupoFarmacologico", "GrupoFarmacologico", true);
                    return View(model);
                }
            }
            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(serviceGrupoFarmacologico.ObtenerTodos(), "idGrupoFarmacologico", "GrupoFarmacologico", true, model.idGrupoFarmacologico);
            return View(model);
        }

        // GET: CatSustancia/Delete/5
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

            if(existencia == 0)
            {
                ViewBag.visiblebtnEliminar=true;
            }


            return View(model);
        }

        // POST: CatSustancia/Delete/5
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
                //if (serviceGrupoFarmacologico != null)
                //{
                //    serviceGrupoFarmacologico.Dispose();
                //    serviceGrupoFarmacologico = null;
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
