
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacen.Web.Logica;
using Almacen.Web.ViewModels;
using Almacen.Infraestructura;
using Almacen.Data;
using Almacen.Web.Applicacion;
using Almacen.Web.ViewModels.Catalogos;
using Almacen.Web.Aplicacion;
using Almacen.Web.Tools;
using RedWillow.MvcToastrFlash;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles =EnumRol.Administrador)]
    public class AlertasController : Controller
    {
        
        private AlertasServicio<Alerta,tAlerta> service = null;

        public AlertasController()
        {
            service = new AlertasServicio<Alerta, tAlerta>(new CndAlmacenDopajeEntities());
        }

        public AlertasController(AlertasServicio<Alerta, tAlerta> catalogService)
        {
            this.service = catalogService;
        }

        #region AlertasGeneral
        // GET: Alertas
        public ActionResult Index()
        {
            AlertasVM model = new AlertasVM();
            //model.ListaAlertas = DatosDemoSustancia.Alertas();
            model.Lista = service.ObtenerTodos();

            return View(model);
        }

        // GET: Alertas/Details/5
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

        // GET: Alertas/Create
        public ActionResult Create()
        {
            List<AlertaUnidadVM> ListaAlertaUnidad = new List<AlertaUnidadVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                
                ListaAlertaUnidad = new LeerCatalogosServicio<AlertaUnidadVM, tAlertaUnidad>(db).ObtenerTodos();
            }

            ViewBag.idAlertaUnidad = CustomExtension.convertSelectList(ListaAlertaUnidad, "idAlertaUnidad", "AlertaUnidad", true);

            return View();
        }

        // POST: Alertas/Create
        [HttpPost]
        public ActionResult Create(Alerta model)
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

            List<AlertaUnidadVM> ListaAlertaUnidad = new List<AlertaUnidadVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaAlertaUnidad = new LeerCatalogosServicio<AlertaUnidadVM, tAlertaUnidad>(db).ObtenerTodos();
            }

            ViewBag.idAlertaUnidad = CustomExtension.convertSelectList(ListaAlertaUnidad, "idAlertaUnidad", "AlertaUnidad", true);

            return View(model);
        }

        // GET: Alertas/Edit/5
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

            List<AlertaUnidadVM> ListaAlertaUnidad = new List<AlertaUnidadVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaAlertaUnidad = new LeerCatalogosServicio<AlertaUnidadVM, tAlertaUnidad>(db).ObtenerTodos();
            }

            ViewBag.idAlertaUnidad = CustomExtension.convertSelectList(ListaAlertaUnidad, "idAlertaUnidad", "AlertaUnidad", true);

            return View(model);
        }

        // POST: Alertas/Edit/5
        [HttpPost]
        public ActionResult Edit(Alerta model)
        {
            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.Editar(model, model.idAlerta);
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

            List<AlertaUnidadVM> ListaAlertaUnidad = new List<AlertaUnidadVM>();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                ListaAlertaUnidad = new LeerCatalogosServicio<AlertaUnidadVM, tAlertaUnidad>(db).ObtenerTodos();
            }

            ViewBag.idAlertaUnidad = CustomExtension.convertSelectList(ListaAlertaUnidad, "idAlertaUnidad", "AlertaUnidad", true);

            return View(model);
        }

        // GET: Alertas/Delete/5
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

        // POST: Alertas/Delete/5
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
        #endregion

        //** Alertas por Sustancia o Material**/

        #region AlertasSustanciaMaterial

        public ActionResult EspecificarSustancias(int? id)
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

            model.tAlertaXSustanciaMaterial=model.tAlertaXSustanciaMaterial.Where(w => w.ST == true && w.idCategoria ==(int)EnumCategoria.Sustancias).ToList();

            List<int> listaActual = model.tAlertaXSustanciaMaterial.Select(s=> s.idSustanciaMaterial).ToList();

            List<SustanciaVM> ListaSustancias = new List<SustanciaVM>();
            

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaSustancias = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>(db).ObtenerTodos(w=> !listaActual.Contains(w.idSustanciaMaterial));
                
            }

            ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaSustancias, "idSustanciaMaterial", "Descripcion");
            

            return View(model);
        }

        [HttpPost]
        public ActionResult EspecificarSustancias(int idAlerta,int idSustanciaMaterial, int Valor, int? Valor100referencia)
        {

            AlertaXSustanciaMaterialVM model = new AlertaXSustanciaMaterialVM();

            model.idAlerta = idAlerta;
            model.idCategoria = (int)EnumCategoria.Sustancias;
            model.idSustanciaMaterial = idSustanciaMaterial;
            model.Valor = Valor;

            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.AgregarSustanciaMaterial(model);
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
                    return RedirectToAction("EspecificarSustancias", new { id = idAlerta });

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

                    return RedirectToAction("EspecificarSustancias", new { id = idAlerta });
                }
            }

            // no deberia llegar aqui
            return View();
        }

        public ActionResult EspecificarMateriales(int? id)
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

            model.tAlertaXSustanciaMaterial = model.tAlertaXSustanciaMaterial.Where(w => w.ST==true && w.idCategoria == (int)EnumCategoria.Materiales).ToList();

            List<int> listaActual = model.tAlertaXSustanciaMaterial.Select(s => s.idSustanciaMaterial).ToList();

            List<MaterialVM> ListaMateriales = new List<MaterialVM>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaMateriales = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>(db).ObtenerTodos(w=> !listaActual.Contains(w.idSustanciaMaterial));
            }

            ViewBag.idSustanciaMaterial = CustomExtension.convertSelectList(ListaMateriales, "idSustanciaMaterial", "Descripcion");

            return View(model);
        }

        [HttpPost]
        public ActionResult EspecificarMateriales(int idAlerta, int idSustanciaMaterial, int Valor, int? Valor100referencia)
        {

            AlertaXSustanciaMaterialVM model = new AlertaXSustanciaMaterialVM();

            model.idAlerta = idAlerta;
            model.idCategoria = (int)EnumCategoria.Materiales;
            model.idSustanciaMaterial = idSustanciaMaterial;
            model.Valor = Valor;
            model.Valor100referencia = Valor100referencia;

            string mensaje = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    service.AgregarSustanciaMaterial(model);
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
                    return RedirectToAction("EspecificarMateriales", new { id = idAlerta });

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

                    return RedirectToAction("EspecificarMateriales", new { id = idAlerta });
                }
            }

            // no deberia llegar aqui
            return View();
        }


        //public ActionResult EspecificarDetails(int? id)
        //{

        //    if (id == null)
        //    {
        //        return RedirectToAction("BadRequest", "Error", null);
        //    }
        //    var model = service.ObtenerPorIdSustanciaMaterial(id);
        //    if (model == null)
        //    {
        //        return RedirectToAction("NotFound", "Error", null);
        //    }

        //    return View(model);
        //}

        public ActionResult EspecificarEdit(int id)
        {
            
            var model = service.ObtenerPorIdSustanciaMaterial(id);

            return View(model);
        }
        [HttpPost]
        public ActionResult EspecificarEdit(int idAlertaXSustanciaMaterial, int ValorEdit, int? Valor100referenciaEdit)
        {
            string mensaje = "";

            var model = new AlertaXSustanciaMaterialVM();

            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                   model= service.EditarSustanciaMaterial(ValorEdit, Valor100referenciaEdit.Value, idAlertaXSustanciaMaterial);
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
                    //return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    this.Flash(Toastr.WARNING, "Error", mensaje, new { timeOut = 0, extendedTimeOut = 0 });

                    //return View(model);
                }
            }

            if (model.idCategoria == (int)EnumCategoria.Sustancias)
            {
                return RedirectToAction("EspecificarSustancias", new { id = model.idAlerta });
            }
            else
            {
                return RedirectToAction("EspecificarMateriales", new { id = model.idAlerta });
            }
        }

        //public ActionResult EspecificarDelete(int? id)
        //{

        //    if (id == null)
        //    {
        //        return RedirectToAction("BadRequest", "Error", null);
        //    }

        //    var model = service.ObtenerPorIdSustanciaMaterial(id);
        //    if (model == null)
        //    {
        //        return RedirectToAction("NotFound", "Error", null);
        //    }
        //    return View(model);
        //}

        
        public ActionResult EspecificarDelete(int id, int idAlerta, int idCategoria)
        {

            try
            {
                // TODO: Add delete logic here
                service.EliminarSustanciaMaterial(id);
                var result = service.Save();
                if(idCategoria==(int) EnumCategoria.Sustancias)
                {
                    return RedirectToAction("EspecificarSustancias", new { id= idAlerta });
                }else
                {
                    return RedirectToAction("EspecificarMateriales", new { id = idAlerta });
                }

            }
            catch( Exception ex)
            {
                string msg = ex.Message;
                return View();
            }
        }
        #endregion


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
