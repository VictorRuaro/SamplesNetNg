using Almacen.Data;
using Almacen.Infraestructura;
using Almacen.Web.Aplicacion;
using Almacen.Web.Applicacion;
using Almacen.Web.Models;
using Almacen.Web.ViewModels;
using Almacen.Web.ViewModels.Catalogos;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web.Controllers
{
    public class DatosAplicacionController : Controller
    {

        // GET: UsuariosJson
        [AllowAnonymous]
        public JsonResult ObtenerUsuario(string idUsuario)
        {
            Usuario result = new Usuario();

            if (!string.IsNullOrEmpty(idUsuario))
            {

                Usuario usr = new Usuario();
                using (LeerCatalogosServicio<Usuario, AspNetUsers> db = new LeerCatalogosServicio<Usuario, AspNetUsers>())
                {
                    usr = db.ObtenerPorId(idUsuario);
                }
                result = usr;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        public JsonResult FirmarUsuario(string usuario,string password)
        {
            Usuario result = new Usuario(); ;

            if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(password))
            {
                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                ApplicationUser existe = UserManager.Find(usuario,password);

                if (existe != null)
                {

                    result.id = existe.Id;
                    result.NombreCompleto = existe.NombreCompleto;
                    result.Email = existe.Email;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult DatosUsuario(string idUsuario)
        {
            Usuario result = new Usuario();

            if (string.IsNullOrEmpty(idUsuario))
            {
                return Content("");
            }

            Usuario model = new Usuario();
            using (LeerCatalogosServicio<Usuario, AspNetUsers> db = new LeerCatalogosServicio<Usuario, AspNetUsers>())
            {
                model = db.ObtenerPorId(idUsuario);
            }

            if (model == null)
            {
                return Content("");

            }
            result = model;

            return PartialView("_DatosUsuario", result);
        }
        [AllowAnonymous]
        public ActionResult DatosSustancia(int? idSustanciaMaterial)
        {
            SustanciaVM result = new SustanciaVM();

            if (idSustanciaMaterial == null)
            {
                return Content("");
            }

            SustanciaVM model = new SustanciaVM();
            using (LeerCatalogosServicio<SustanciaVM, tDetalleSustacia> db = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>())
            {
                model = db.ObtenerPorId(idSustanciaMaterial);
            }

            if (model == null)
            {
                return Content("");

            }
            result = model;
            return PartialView("_DatosSustancia", result);
        }

        [AllowAnonymous]
        public ActionResult DatosPresentacionSustancia(int? idAlmacen)
        {
            AlmacenSustanciaVM result = new AlmacenSustanciaVM();

            if (idAlmacen == null)
            {
                return Content("");
            }

            AlmacenSustanciaVM model = new AlmacenSustanciaVM();
            using (LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia> db = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>())
            {
                model = db.ObtenerPorId(idAlmacen);
            }

            if (model == null)
            {
                return Content("");

            }
            result = model;
            return PartialView("_DatosPresentacionSustancia", result);
        }

        [AllowAnonymous]
        public ActionResult DatosPresentacionMaterial(int? idAlmacen)
        {
            AlmacenMaterialVM result = new AlmacenMaterialVM();

            if (idAlmacen == null)
            {
                return Content("");
            }

            AlmacenMaterialVM model = new AlmacenMaterialVM();
            using (LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial> db = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>())
            {
                model = db.ObtenerPorId(idAlmacen);
            }

            if (model == null)
            {
                return Content("");

            }
            result = model;
            return PartialView("_DatosPresentacionMaterial", result);
        }

        [AllowAnonymous]
        public ActionResult Sustancia(int? idSustanciaMaterial)
        {
            SustanciaVM result = new SustanciaVM();

            if (idSustanciaMaterial == null)
            {
                return Content("");
            }

            SustanciaVM model = new SustanciaVM();
            using (LeerCatalogosServicio<SustanciaVM, tDetalleSustacia> db = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>())
            {
                model = db.ObtenerPorId(idSustanciaMaterial);
            }

            if (model == null)
            {
                return Content("");

            }
            result = model;
            return PartialView("_Sustancia", result);
        }
        [AllowAnonymous]
        public ActionResult Material(int? idSustanciaMaterial)
        {
            MaterialVM result = new MaterialVM();

            if (idSustanciaMaterial == null)
            {
                return Content("");
            }

            MaterialVM model = new MaterialVM();
            using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            {
                model = db.ObtenerPorId(idSustanciaMaterial);
            }

            if (model == null)
            {
                return Content("");

            }
            result = model;
            return PartialView("_Material", result);
        }

        [AllowAnonymous]
        public ActionResult ExistenciasSustancia(int? idSustanciaMaterial)
        {
            int result = 0;

            if (idSustanciaMaterial == null)
            {
                return Content("");
            }

            using (SustanciasServicio servicio = new SustanciasServicio(new CndAlmacenDopajeEntities()))
            {
                result = servicio.Existencias(idSustanciaMaterial.Value, w => w.idSustanciaMaterial == idSustanciaMaterial);
            }

            return PartialView("_ExistenciaSustancia", result);
        }

        [AllowAnonymous]
        public ActionResult DatosMaterial(int? idSustanciaMaterial)
        {
            MaterialVM result = new MaterialVM();

            if (idSustanciaMaterial == null)
            {
                return Content("");
            }

            MaterialVM model = new MaterialVM();
            using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            {
                model = db.ObtenerPorId(idSustanciaMaterial);
            }

            if (model == null)
            {
                return Content("");

            }
            result = model;
            return PartialView("_DatosMaterial", result);
        }

        [AllowAnonymous]
        public ActionResult ExistenciasMaterial(int? idSustanciaMaterial)
        {
            int result = 0;

            if (idSustanciaMaterial == null)
            {
                return Content("");
            }

            using (MaterialesServicio servicio= new MaterialesServicio(new CndAlmacenDopajeEntities()))
            {
                result = servicio.Existencias(idSustanciaMaterial.Value,w => w.idSustanciaMaterial== idSustanciaMaterial);
            }

            return PartialView("_ExistenciaMaterial", result);
        }

        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        public JsonResult ObtenerExistencias(int Cantidad, int? idAlmacen, int? idCategoria)
        {
            bool result = false;

            int existencia = 0;

            if (idAlmacen.HasValue)
            {

                var lista=LogicaAplicacion.ObtenerExistenciasPresentacion(idCategoria.Value, idAlmacen.Value);

                if (lista.Any())
                {
                    existencia = LogicaAplicacion.ExistenciasPorFechaLote(lista, lista.FirstOrDefault().idSustanciaMaterial, true);
                }else
                {
                    existencia = 0;
                }

                result = LogicaAplicacion.ValidaExistencia(Cantidad, existencia);
                
            }
            else
            {
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DetalleLote(int idCategoria, int? idAlmacen)
        {
            

            if (idAlmacen == null)
            {
                return Content("");
            }
            if (idCategoria == (int)EnumCategoria.Sustancias)
            {
                AlmacenSustanciaVM model = new AlmacenSustanciaVM();
                using (LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia> db = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>())
                {
                    model = db.ObtenerPorId(idAlmacen);
                }

                if (model == null)
                {
                    return Content("");
                }

                return PartialView("_DatosLoteSustancia", model);
            }
            else
            {
                AlmacenMaterialVM model = new AlmacenMaterialVM();
                using (LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial> db = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>())
                {
                    model = db.ObtenerPorId(idAlmacen);
                }

                if (model == null)
                {
                    return Content("");
                }

                return PartialView("_DatosLoteMaterial", model);
            }

            
        }


    }
}