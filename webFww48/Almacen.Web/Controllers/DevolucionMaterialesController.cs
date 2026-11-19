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
using System.Web.Mvc;
using System.Web.Routing;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador + "," + EnumRol.Administrador_Informacion + "," + EnumRol.Administrador_Almacen)]
    public class DevolucionMaterialesController : Controller
    {
        private DevolucionMaterialesServicio service = null;
        public DevolucionMaterialesController()
        {
            service = new DevolucionMaterialesServicio(new CndAlmacenDopajeEntities());
        }

        public DevolucionMaterialesController(DevolucionMaterialesServicio catalogService)
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

        // GET: AlmacenMateriales
        public ActionResult Index(MaterialFiltrosVM Filtros = null, int pagina = 1)
        {

            BaseListaCatalogosVM<MaterialVM> model = new BaseListaCatalogosVM<MaterialVM>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = 10;

            List<GradoVM> ListaGrado = new List<GradoVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrado = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();
            }

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrado, "idGrado", "Grado", true, Filtros.idGrado);

            var parametros = new ParametrosQuery<tDetalleMaterial>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

            var query = PredicateBuilder.True<tDetalleMaterial>();
            System.Linq.Expressions.ExpressionType cNodeType = query.Body.NodeType;
            if (Filtros != null)
            {

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
                if (!string.IsNullOrEmpty(Filtros.NumeroParte))
                {
                    query = query.And(w => w.Descripcion.Contains(Filtros.NumeroParte));
                }
            }

            if (query.Body.NodeType != cNodeType)
            {
                parametros.Where = query;
            }

            model = service.ObtenerPaginado(parametros);

            if (query.Body.NodeType == cNodeType && model.Paginacion.TotalRegistros == 0) // si no hay filtros de busqueda y no hay elementos en la lista 
            {
                model.SinDatosListado = true;
            }

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Codigo"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["idGrado"] = Filtros.idGrado;
            model.Paginador.ValoresQueryString["NumeroParte"] = Filtros.NumeroParte;

            ViewBag.MaterialFiltros = Filtros;

            return View(model);
        }

        public ActionResult Existencias(int idSustanciaMaterial)
        {
            BaseListaAlmacenVM<MaterialVM, DevolucionMaterialVM> model = new BaseListaAlmacenVM<MaterialVM, DevolucionMaterialVM>();

            model = service.ObtenerExistencias(idSustanciaMaterial, w => w.idSustanciaMaterial == idSustanciaMaterial);
            //ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true);
            return View(model);
        }

        // GET: AlmacenMateriales/Details/5
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

        // GET: AlmacenMateriales/Create
        public ActionResult Create(int? idSustanciaMaterial)
        {
            DevolucionMaterialVM model = new DevolucionMaterialVM();

            //List<MarcaVM> ListaMarca = new List<MarcaVM>();
            //List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();
            //List<CondicionesAlmacenajeVM> ListaCAlmacenaje = new List<CondicionesAlmacenajeVM>();
            //List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            List<MaterialVM> ListaMateriales = new List<MaterialVM>();
            List<AlmacenVM> ListaLote = new List<AlmacenVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                if (idSustanciaMaterial.HasValue)
                {
                    ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                        .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == idSustanciaMaterial);

                    model.tSustanciaMaterial = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db)
                        .ObtenerPorId(idSustanciaMaterial.Value);

                    model.idSustanciaMaterial = idSustanciaMaterial.Value;
                    ListaMateriales = null;
                }
                else
                {
                    ListaMateriales = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).EncontrarPor(w => w.tAlmacen.Where(a => a.ST == true).Any());
                    ListaLote = new List<AlmacenVM>() { new AlmacenVM { idAlmacen = 0, LoteInterno = "" } };
                    model = null;
                }
            }

            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");

            if (ListaMateriales != null)
            {
                ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaMateriales, "idSustanciaMaterial", "Descripcion");
            }

            return View(model);
        }

        // POST: AlmacenMateriales/Create
        [HttpPost]
        public ActionResult Create(DevolucionMaterialVM model, string post)
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
                    return RedirectToAction("Existencias", new { idSustanciaMaterial = model.idSustanciaMaterial });

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });
                    //ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true, model.idGrado);
                    //return View(model);
                }
            }

            //List<MarcaVM> ListaMarca = new List<MarcaVM>();
            //List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();
            //List<CondicionesAlmacenajeVM> ListaCAlmacenaje = new List<CondicionesAlmacenajeVM>();
            //List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            List<MaterialVM> ListaMateriales = new List<MaterialVM>();

            List<AlmacenVM> ListaLote = new List<AlmacenVM>();

            AlmacenMaterialVM lote = new AlmacenMaterialVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                if (model.idSustanciaMaterial > 0)
                {
                    ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                        .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);

                    model.tSustanciaMaterial = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerPorId(model.idSustanciaMaterial);

                    model.idSustanciaMaterial = model.idSustanciaMaterial;
                    ListaMateriales = null;

                    if (model.idAlmacen > 0)
                    {
                        lote = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>(db)
                                .EncontrarPor(w => w.idAlmacen == model.idAlmacen).FirstOrDefault();
                    }
                }
                else
                {
                    ListaMateriales = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).EncontrarPor(w => w.tAlmacen.Where(a => a.ST == true).Any());
                    ListaLote = new List<AlmacenVM>() { new AlmacenVM { idAlmacen = 0, LoteInterno = "" } };
                    model = null;
                    
                }
            }

            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");
            ViewBag.Lote = lote;

            if (ListaMateriales != null)
            {
                ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaMateriales, "idSustanciaMaterial", "Descripcion");
            }

            return View(model);
        }

        [AllowAnonymous]
        public JsonResult DatosMaterial(string idMaterial)
        {
            int idSustanciaMaterial = int.Parse(idMaterial);

            //int GenNumDeLoteInterno = service.GenNumDeLoteInterno(idSustanciaMaterial);

            MaterialVM model = new MaterialVM();
            List<AlmacenVM> ListaLote = new List<AlmacenVM>();

            using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            {
                model = db.ObtenerPorId(idSustanciaMaterial);
            }

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                model = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db)
                    .ObtenerPorId(idSustanciaMaterial);

                ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                    .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);
            }

            //string rLoteInterno = string.Format("{0}-{1:D4}", model.Codigo, GenNumDeLoteInterno);

            return Json(new {
                Codigo = model.Codigo,
                Grado = model.cGrado.Grado,
                Lotes = ListaLote.Select(s => new { value = s.idAlmacen, text = s.LoteInterno }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }

        [AllowAnonymous]
        public ActionResult DatosLoteMaterial(int idAlmacen)
        {

            AlmacenMaterialVM model = new AlmacenMaterialVM();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                model = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>(db)
                    .EncontrarPor(w => w.idAlmacen == idAlmacen).FirstOrDefault();
            }
            return PartialView("_LoteDevolucion", model);

        }
        // GET: AlmacenMateriales/Edit/5
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

            List<AlmacenVM> ListaLote = new List<AlmacenVM>();
            AlmacenMaterialVM lote = new AlmacenMaterialVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                    .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);

                if (model.idAlmacen > 0)
                {
                    lote = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>(db)
                            .EncontrarPor(w => w.idAlmacen == model.idAlmacen).FirstOrDefault();
                }

            }

            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");

            ViewBag.Lote = lote;

            return View(model);
        }

        // POST: AlmacenMateriales/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, DevolucionMaterialVM model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idDevolucion);
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
                    return RedirectToAction("Existencias", new { idSustanciaMaterial = model.idSustanciaMaterial });

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

                }
            }

            List<AlmacenVM> ListaLote = new List<AlmacenVM>();
            AlmacenMaterialVM lote = new AlmacenMaterialVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                    .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);

                if (model.idAlmacen > 0)
                {
                    lote = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>(db)
                            .EncontrarPor(w => w.idAlmacen == model.idAlmacen).FirstOrDefault();
                }

                model.tSustanciaMaterial = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerPorId(model.idSustanciaMaterial);
                model.idSustanciaMaterial = model.idSustanciaMaterial;
            }


            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");

            ViewBag.Lote = lote;

            return View(model);
        }

        // GET: AlmacenMateriales/Delete/5
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

        // POST: AlmacenMateriales/Delete/5
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
