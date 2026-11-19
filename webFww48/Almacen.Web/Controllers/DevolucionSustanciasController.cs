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
using AutoMapper;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador + "," + EnumRol.Administrador_Informacion + "," + EnumRol.Administrador_Almacen)]
    public class DevolucionSustanciasController : Controller
    {
        private DevolucionSustanciasServicio service = null;
        public DevolucionSustanciasController()
        {
            service = new DevolucionSustanciasServicio(new CndAlmacenDopajeEntities());
        }

        public DevolucionSustanciasController(DevolucionSustanciasServicio catalogService)
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
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico>(db).ObtenerTodos();

            }

            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrupoFarmacologico", "GrupoFarmacologico", true, Filtros.idGrupoFarmacologico);

            var parametros = new ParametrosQuery<tDetalleSustacia>(model.Paginador.PaginaActual, model.Paginador.RegistrosPorPagina);

            var query = PredicateBuilder.True<tDetalleSustacia>();
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

                if (Filtros.idGrupoFarmacologico > 0)
                {
                    query = query.And(w => w.idGrupoFarmacologico == Filtros.idGrupoFarmacologico);
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
                    query = query.And(w => w.EsSustanciaControlada == Filtros.EsSustanciaControlada);
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
            BaseListaAlmacenVM<SustanciaVM, DevolucionSustanciaVM> model = new BaseListaAlmacenVM<SustanciaVM, DevolucionSustanciaVM>();

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
            DevolucionSustanciaVM model = new DevolucionSustanciaVM();

            List<SustanciaVM> ListaSustancias = new List<SustanciaVM>();

            List<AlmacenVM> ListaLote = new List<AlmacenVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                if (idSustanciaMaterial.HasValue)
                {
                    ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                        .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == idSustanciaMaterial);

                    model.tSustanciaMaterial = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db)
                        .ObtenerPorId(idSustanciaMaterial.Value);

                    model.idSustanciaMaterial = idSustanciaMaterial.Value;
                    ListaSustancias = null;
                }
                else
                {

                    ListaSustancias = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).EncontrarPor(w => w.tAlmacen.Where(a => a.ST == true).Any());
                    ListaLote = new List<AlmacenVM>() { new AlmacenVM { idAlmacen = 0, LoteInterno ="" } };
                    model = null;
                }
            }

            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");

            if (ListaSustancias != null)
            {
                ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaSustancias, "idSustanciaMaterial", "Descripcion");
            }

            return View(model);
        }

        // POST: AlmacenSustancias/Create
        [HttpPost]
        public ActionResult Create(DevolucionSustanciaVM model, string post)
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

            List<SustanciaVM> ListaSustancias = new List<SustanciaVM>();

            List<AlmacenVM> ListaLote = new List<AlmacenVM>();

            AlmacenSustanciaVM lote = new AlmacenSustanciaVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                if (model.idSustanciaMaterial > 0)
                {
                    ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                        .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);

                    model.tSustanciaMaterial = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db)
                        .ObtenerPorId(model.idSustanciaMaterial);

                    model.idSustanciaMaterial = model.idSustanciaMaterial;
                    ListaSustancias = null;

                    if (model.idAlmacen>0)
                    {
                        lote = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>(db)
                                .EncontrarPor(w => w.idAlmacen == model.idAlmacen).FirstOrDefault();
                    }
                }
                else
                {
                    ListaSustancias = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).EncontrarPor( w=> w.tAlmacen.Where(a=> a.ST==true).Any());
                    ListaLote = new List<AlmacenVM>() { new AlmacenVM { idAlmacen = 0, LoteInterno = "" } };
                    model = null;
                }
            }

            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");
            ViewBag.Lote = lote;


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

            //int GenNumDeLoteInterno = service.GenNumDeLoteInterno(idSustanciaMaterial);

            SustanciaVM model = new SustanciaVM();
            List<AlmacenVM> ListaLote = new List<AlmacenVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                model = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db)
                    .ObtenerPorId(idSustanciaMaterial);

                ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db)
                    .EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);
            }

            //string rLoteInterno = string.Format("{0}-{1:D4}", model.Codigo, GenNumDeLoteInterno);

            return Json(new
            {
                Codigo = model.Codigo,
                CAS = model.CAS,
                PMCompuesto = model.PMCompuesto,
                FormulaMolecular = model.FormulaMolecular,
                GrupoFarmacologico = model.cGrupoFarmacologico.GrupoFarmacologico,
                colorHtml = model.cGrupoFarmacologico.colorHtml,
                EsSustanciaControlada=model.EsSustanciaControlada?"Si":"No",
                Lotes = ListaLote.Select(s=> new { value=s.idAlmacen, text=s.LoteInterno }).ToList()
                //LoteInterno = rLoteInterno,
                //NumDeLoteInterno = GenNumDeLoteInterno
            }
            , JsonRequestBehavior.AllowGet);

        }

        [AllowAnonymous]
        public ActionResult DatosLoteSustancia(int idAlmacen)
        {

            AlmacenSustanciaVM model = new AlmacenSustanciaVM();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                model = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>(db)
                    .EncontrarPor(w=> w.idAlmacen == idAlmacen).FirstOrDefault();
            }
            return PartialView("_LoteDevolucion",model);

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


            List<AlmacenVM> ListaLote = new List<AlmacenVM>();
            AlmacenSustanciaVM lote = new AlmacenSustanciaVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db).EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);

                if (model.idAlmacen > 0)
                {
                    lote = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>(db)
                            .EncontrarPor(w => w.idAlmacen == model.idAlmacen).FirstOrDefault();
                }
            }

            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");

            ViewBag.Lote = lote;

            return View(model);
        }

        // POST: AlmacenSustancias/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, DevolucionSustanciaVM model)
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
            AlmacenSustanciaVM lote = new AlmacenSustanciaVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaLote = new LeerCatalogosServicio<AlmacenVM, tAlmacen>(db).EncontrarPor(w => w.ST == true && w.idSustanciaMaterial == model.idSustanciaMaterial);

                if (model.idAlmacen > 0)
                {
                    lote = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>(db)
                            .EncontrarPor(w => w.idAlmacen == model.idAlmacen).FirstOrDefault();
                }

                model.tSustanciaMaterial = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).ObtenerPorId(model.idSustanciaMaterial);
                model.idSustanciaMaterial = model.idSustanciaMaterial;
            }

            ViewBag.idAlmacen = CustomExtension.convertSelectList(ListaLote, "idAlmacen", "LoteInterno");

            ViewBag.Lote = lote;


            return View(model);
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
