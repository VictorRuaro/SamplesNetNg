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
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador + "," + EnumRol.Administrador_Informacion + "," + EnumRol.Administrador_Almacen)]
    public class AlmacenSustanciasController : Controller
        {
            private SustanciasServicio service = null;
            public AlmacenSustanciasController()
            {
                service = new SustanciasServicio(new CndAlmacenDopajeEntities());
            }

            public AlmacenSustanciasController(SustanciasServicio catalogService)
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

            // GET: AlmacenSustancias
            public ActionResult Index(SustanciaFiltrosVM Filtros = null, int pagina = 1)
            {

                BaseListaCatalogosVM<SustanciaVM> model = new BaseListaCatalogosVM<SustanciaVM>();
                model.Paginador.PaginaActual = pagina;
                model.Paginador.RegistrosPorPagina = 10;

                List<GrupoFarmacologicoVM> ListaGrupoFarmacologico = new List<GrupoFarmacologicoVM>();

                using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
                {
                    ListaGrupoFarmacologico = new LeerCatalogosServicio<GrupoFarmacologicoVM,cGrupoFarmacologico>(db).ObtenerTodos();

                }

                ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrupoFarmacologico", "GrupoFarmacologico", true, Filtros.idGrupoFarmacologico);

                var parametros = new ParametrosQuery<tDetalleSustacia>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

                var query = PredicateBuilder.True<tDetalleSustacia>();
                System.Linq.Expressions.ExpressionType  cNodeType = query.Body.NodeType;
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

                if (Filtros.idGrupoFarmacologico > 0)
                {
                    query = query.And(w => w.idGrupoFarmacologico == Filtros.idGrupoFarmacologico);
                }

                if (!string.IsNullOrEmpty(Filtros.Sinonimos))
                {
                    query = query.And(w => w.Sinonimos.Contains(Filtros.Sinonimos));
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

                if (Filtros.EsSustanciaControlada.HasValue)
                {
                    query = query.And(w => w.EsSustanciaControlada==Filtros.EsSustanciaControlada);
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
                model.Paginador.ValoresQueryString["idGrupoFarmacologico"] = Filtros.idGrupoFarmacologico;
                model.Paginador.ValoresQueryString["Sinonimos"] = Filtros.Sinonimos;
                model.Paginador.ValoresQueryString["CAS"] = Filtros.CAS;
                model.Paginador.ValoresQueryString["PMCompuesto"] = Filtros.PMCompuesto;
                model.Paginador.ValoresQueryString["FormulaMolecular"] = Filtros.FormulaMolecular;

                ViewBag.SustanciaFiltros = Filtros;

                return View(model);
            }

            public ActionResult Existencias(int idSustanciaMaterial)
            {
                BaseListaAlmacenVM<SustanciaVM, AlmacenSustanciaVM> model = new BaseListaAlmacenVM<SustanciaVM, AlmacenSustanciaVM>();

                model = service.ObtenerExistencias(idSustanciaMaterial, w => w.idSustanciaMaterial == idSustanciaMaterial);
                //ViewBag.idGrado = CustomExtension.convertSelectList(serviceGrado.ObtenerTodos(), "idGrado", "Grado", true);
                return View(model);
            }

            // GET: AlmacenSustancias/Details/5
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

            // GET: AlmacenSustancias/Create
            public ActionResult Create(int? idSustanciaMaterial)
            {
                AlmacenSustanciaVM model = new AlmacenSustanciaVM();


            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            List<SustanciaVM> ListaSustancias = new List<SustanciaVM>();

                using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
                {

                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();

                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);



                if (idSustanciaMaterial.HasValue)
                    {
                        int GenNumDeLoteInterno = service.GenNumDeLoteInterno(idSustanciaMaterial.Value);

                        model.tSustanciaMaterial = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).ObtenerPorId(idSustanciaMaterial.Value);

                        string rLoteInterno = string.Format("{0}-{1:D4}", model.tSustanciaMaterial.Codigo, GenNumDeLoteInterno);

                        model.LoteInterno = rLoteInterno;
                        model.NumDeLoteInterno = GenNumDeLoteInterno;
                        model.idSustanciaMaterial = idSustanciaMaterial.Value;
                        ListaSustancias = null;
                    }
                    else
                    {
                    ListaSustancias = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).ObtenerTodos();
                    //ListaSustancias = new LeerCatalogosServicio<SustanciaVM,tDetalleSustacia>(db).EncontrarPor(w=> w.tAlmacen.Where(a => a.ST == true).Any());
                        model = null;
                    }
                }


                ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
                ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
                ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");

                ViewBag.idUMCantidadEnvase = CustomExtension.convertSelectList(ListaUMCantidadEnvase, "idUMCantidadEnvase", "UMCantidadEnvase");
                ViewBag.idUMConcentracion  = CustomExtension.convertSelectList(ListaUMConcentracion, "idUMConcentracion", "UMConcentracion",true);
                ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");
                ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");
            
                ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");


                if (ListaSustancias != null)
                {
                    ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaSustancias, "idSustanciaMaterial", "Descripcion");
                }

                return View(model);
            }

            // POST: AlmacenSustancias/Create
            [HttpPost]
            public ActionResult Create(AlmacenSustanciaVM model, string post)
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

            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            List<SustanciaVM> ListaSustancias = new List<SustanciaVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();

                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);



                if (model.idSustanciaMaterial>0)
                {
                    int GenNumDeLoteInterno = service.GenNumDeLoteInterno(model.idSustanciaMaterial);

                    model.tSustanciaMaterial = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).ObtenerPorId(model.idSustanciaMaterial);

                    string rLoteInterno = string.Format("{0}-{1:D4}", model.tSustanciaMaterial.Codigo, GenNumDeLoteInterno);

                    model.LoteInterno = rLoteInterno;
                    model.NumDeLoteInterno = GenNumDeLoteInterno;
                    model.idSustanciaMaterial = model.idSustanciaMaterial;
                    ListaSustancias = null;
                }
                else
                {
                    ListaSustancias = new LeerCatalogosServicio<SustanciaVM,tDetalleSustacia>(db).ObtenerTodos();
                    model = null;
                }

            }


            ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
            ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
            ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");

            ViewBag.idUMCantidadEnvase = CustomExtension.convertSelectList(ListaUMCantidadEnvase, "idUMCantidadEnvase", "UMCantidadEnvase");
            ViewBag.idUMConcentracion = CustomExtension.convertSelectList(ListaUMConcentracion, "idUMConcentracion", "UMConcentracion", true);
            ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");
            ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");

            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");


            if (ListaSustancias != null)
            {
                ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaSustancias, "idSustanciaMaterial", "Descripcion");
            }

            return View(model);
            }

            [AllowAnonymous]
            public JsonResult DatosSustancia(string idSustancia)
            {
                int idSustanciaMaterial = int.Parse(idSustancia);

                int GenNumDeLoteInterno = service.GenNumDeLoteInterno(idSustanciaMaterial);

                SustanciaVM model = new SustanciaVM();
                using (LeerCatalogosServicio<SustanciaVM, tDetalleSustacia> db = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>())
                {
                    model = db.ObtenerPorId(idSustanciaMaterial);
                }

                string rLoteInterno = string.Format("{0}-{1:D4}", model.Codigo, GenNumDeLoteInterno);

                return Json(new {
                    Codigo = model.Codigo,
                    CAS = model.CAS,
                    PMCompuesto = model.PMCompuesto,
                    FormulaMolecular = model.FormulaMolecular,
                    GrupoFarmacologico = model.cGrupoFarmacologico.GrupoFarmacologico,
                    colorHtml = model.cGrupoFarmacologico.colorHtml,
                    LoteInterno = rLoteInterno,
                    NumDeLoteInterno = GenNumDeLoteInterno }
                , JsonRequestBehavior.AllowGet);

            }


            // GET: AlmacenSustancias/Edit/5
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


            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();



            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
                {
                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();

                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);

            }


            ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
            ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
            ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");

            ViewBag.idUMCantidadEnvase = CustomExtension.convertSelectList(ListaUMCantidadEnvase, "idUMCantidadEnvase", "UMCantidadEnvase");
            ViewBag.idUMConcentracion = CustomExtension.convertSelectList(ListaUMConcentracion, "idUMConcentracion", "UMConcentracion", true);
            ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");
            ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");

            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            return View(model);
            }

            // POST: AlmacenSustancias/Edit/5
            [HttpPost]
            public ActionResult Edit(int id, AlmacenSustanciaVM model)
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

            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();

            

                using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
                {

                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();

                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoria);

                model.tSustanciaMaterial = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).ObtenerPorId(model.idSustanciaMaterial);
                    model.idSustanciaMaterial = model.idSustanciaMaterial;
                }


            ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
            ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
            ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");

            ViewBag.idUMCantidadEnvase = CustomExtension.convertSelectList(ListaUMCantidadEnvase, "idUMCantidadEnvase", "UMCantidadEnvase");
            ViewBag.idUMConcentracion = CustomExtension.convertSelectList(ListaUMConcentracion, "idUMConcentracion", "UMConcentracion", true);
            ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");
            ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");

            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            return View(model);
            }

        // GET: AlmacenSustancias/Certificado/5
        [HttpGet]
        public ActionResult Certificado(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            var sustancia = service.ObtenerPorId(id);
            if (sustancia == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            CertificadoVM model= new CertificadoVM();
            model.Sustancia = sustancia;


            using (LeerCatalogosServicio<AlmacenCertificadoVM, tAlmacenCertificado> db = new LeerCatalogosServicio<AlmacenCertificadoVM, tAlmacenCertificado>())
            {
                model.Historial = db.ObtenerTodos(w=> w.idAlmacen== model.Sustancia.idAlmacen).OrderByDescending(o=>o.FG).ToList();
            }

            return View(model);
        }

        // POST: AlmacenSustancias/Certificado/5
        [HttpPost]
        public ActionResult Certificado(int idAlmacen, AlmacenCertificadoVM certificado)
        {
            if (idAlmacen == 0 || certificado == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }

            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.AgregarCertificado(idAlmacen, certificado);
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
                    return RedirectToAction("Certificado", new { id = idAlmacen });

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

                }
            }

            CertificadoVM model = new CertificadoVM();
            model.Sustancia = service.ObtenerPorId(idAlmacen);


            if (model.Sustancia == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            using (LeerCatalogosServicio<AlmacenCertificadoVM, tAlmacenCertificado> db = new LeerCatalogosServicio<AlmacenCertificadoVM, tAlmacenCertificado>())
            {
                model.Historial = db.ObtenerTodos(w => w.idAlmacen == model.Sustancia.idAlmacen);
            }

            return View(model);
        }

        public ActionResult DescargarCertificado(int idAlmacenCertificado)
        {
            AlmacenCertificadoVM cert = null;

            cert = service.ObtenerCertificado(idAlmacenCertificado);

            if (cert.Archivo.Length > 0)
            {
                return File(cert.Archivo, System.Net.Mime.MediaTypeNames.Application.Octet, cert.NombreArchivo);
            }

            return null;
        }

        public ActionResult VerCertificado(int idAlmacenCertificado)
        {
            try
            {

                FileStreamResult Result = null;

                AlmacenCertificadoVM cert = null;

                cert = service.ObtenerCertificado(idAlmacenCertificado);

                if (cert.Archivo.Length > 0)
                {

                    MemoryStream archivoStream = new MemoryStream();
                    archivoStream.Write(cert.Archivo, 0, cert.Archivo.Length);
                    archivoStream.Position = 0;

                    Result = new FileStreamResult(archivoStream, System.Net.Mime.MediaTypeNames.Application.Octet);

                }

                Result.FileDownloadName = cert.NombreArchivo;

                return Result;
            }
            catch (Exception ex)
            {
                var es = ex;
                return null;

            }
        }

        public ActionResult BorrarCertificado(int idAlmacen, int idAlmacenCertificado)
        {
            if (idAlmacenCertificado ==0 || idAlmacen==0)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }

            string mensaje = "";

            try
            {
                // TODO: Add insert logic here
                service.EliminarCertificado(idAlmacenCertificado);
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
                

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

            }

            return RedirectToAction("Certificado", new { id = idAlmacen });

        }

        // GET: AlmacenSustancias/Delete/5
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

            // POST: AlmacenSustancias/Delete/5
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


