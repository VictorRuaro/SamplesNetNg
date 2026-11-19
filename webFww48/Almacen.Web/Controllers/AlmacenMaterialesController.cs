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
    [Authorize(Roles = EnumRol.Administrador +","+ EnumRol.Administrador_Informacion + "," + EnumRol.Administrador_Almacen)]
    public class AlmacenMaterialesController : Controller
    {
        private MaterialesServicio service = null;
        public AlmacenMaterialesController()
        {
            service = new MaterialesServicio(new CndAlmacenDopajeEntities());
        }

        public AlmacenMaterialesController(MaterialesServicio catalogService)
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
                ListaGrado = new LeerCatalogosServicio<GradoVM,cGrado>(db).ObtenerTodos();
            }

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrado , "idGrado", "Grado", true, Filtros.idGrado);

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

            if (query.Body.NodeType == cNodeType && model.Paginacion.TotalRegistros==0) // si no hay filtros de busqueda y no hay elementos en la lista 
            {
                model.SinDatosListado =  true;
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
            BaseListaAlmacenVM<MaterialVM,AlmacenMaterialVM> model = new BaseListaAlmacenVM<MaterialVM, AlmacenMaterialVM>();

            model = service.ObtenerExistencias(idSustanciaMaterial, w=> w.idSustanciaMaterial==idSustanciaMaterial);
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
            AlmacenMaterialVM model = new AlmacenMaterialVM();


            List<MarcaVM> ListaMarca = new List<MarcaVM>();
            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();
            List<CondicionesAlmacenajeVM> ListaCAlmacenaje = new List<CondicionesAlmacenajeVM>();
            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            List<MaterialVM> ListaMateriales = new List<MaterialVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM,cPresentacion>(db).ObtenerTodos();
                ListaCAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM,cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM,cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);

                

                if (idSustanciaMaterial.HasValue)
                {
                    int GenNumDeLoteInterno = service.GenNumDeLoteInterno(idSustanciaMaterial.Value);

                    model.tSustanciaMaterial = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerPorId(idSustanciaMaterial.Value);

                    string rLoteInterno = string.Format("{0}-{1:D4}", model.tSustanciaMaterial.Codigo, GenNumDeLoteInterno);

                    model.LoteInterno = rLoteInterno;
                    model.NumDeLoteInterno = GenNumDeLoteInterno;
                    model.idSustanciaMaterial = idSustanciaMaterial.Value;
                    ListaMateriales = null;
                }
                else
                {
                    ListaMateriales = new LeerCatalogosServicio<MaterialVM,tDetalleMaterial>(db).ObtenerTodos();
                    model = null;
                }
            }


            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");
            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");


            if (ListaMateriales!=null)
            {
                ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaMateriales, "idSustanciaMaterial", "Descripcion");
            }

            return View(model);
        }

        // POST: AlmacenMateriales/Create
        [HttpPost]
        public ActionResult Create(AlmacenMaterialVM model,string post)
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

            List<MarcaVM> ListaMarca = new List<MarcaVM>();
            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();
            List<CondicionesAlmacenajeVM> ListaCAlmacenaje = new List<CondicionesAlmacenajeVM>();
            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            List<MaterialVM> ListaMateriales = new List<MaterialVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM, cPresentacion>(db).ObtenerTodos();
                ListaCAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM, cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);

                ListaMateriales = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerTodos();

                model.tSustanciaMaterial = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerPorId(model.idSustanciaMaterial);
                model.idSustanciaMaterial = model.idSustanciaMaterial;
            }

            
            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");
            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaMateriales, "idSustanciaMaterial", "Descripcion");
            
            return View(model);
        }

        [AllowAnonymous]
        public JsonResult DatosMaterial(string idMaterial)
        {
            int idSustanciaMaterial = int.Parse(idMaterial);

            MaterialVM model = new MaterialVM();
            int GenNumDeLoteInterno = service.GenNumDeLoteInterno(idSustanciaMaterial);

            using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            {
                model = db.ObtenerPorId(idSustanciaMaterial);
            }

            string rLoteInterno = string.Format("{0}-{1:D4}", model.Codigo , GenNumDeLoteInterno);

            return Json(new { Codigo=model.Codigo, Grado=model.cGrado.Grado, LoteInterno= rLoteInterno, NumDeLoteInterno = GenNumDeLoteInterno }, JsonRequestBehavior.AllowGet);

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


            List<MarcaVM> ListaMarca = new List<MarcaVM>();
            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();
            List<CondicionesAlmacenajeVM> ListaCAlmacenaje = new List<CondicionesAlmacenajeVM>();
            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM, cPresentacion>(db).ObtenerTodos();
                ListaCAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM, cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);

            }

            
            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");
            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            return View(model);
        }

        // POST: AlmacenMateriales/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, AlmacenMaterialVM model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idAlmacen);
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

            List<MarcaVM> ListaMarca = new List<MarcaVM>();
            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();
            List<CondicionesAlmacenajeVM> ListaCAlmacenaje = new List<CondicionesAlmacenajeVM>();
            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM, cPresentacion>(db).ObtenerTodos();
                ListaCAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM, cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);

                model.tSustanciaMaterial = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerPorId(model.idSustanciaMaterial);
                model.idSustanciaMaterial = model.idSustanciaMaterial;
            }

            
            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");
            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

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
