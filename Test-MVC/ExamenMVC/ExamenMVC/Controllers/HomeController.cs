using Comun;
using ExamenMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ExamenMVC.Controllers
{
    public class HomeController : Controller
    {

        string uri = "https://localhost:44306/Api/";
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ModelUser usuario)
        {
            ActionResult actionToDo = View(usuario);

            try
            {

                if (!IsUserValid(usuario))
                {
                    return actionToDo;
                }

                var resp = Login(usuario);

                if (resp!=null)
                {
                    Session["Usuario"] = resp;

                    actionToDo = RedirectToAction("Main");
                }
                else
                {
                    ViewBag.Msg = "Usuario no valido";
                }

            }
            catch (Exception ex)
            {

                throw;
            }


            return actionToDo;
        }

        public ActionResult Main(ModelMain model)
        {
            var usuario= UserSesion();

            if (usuario == null) return RedirectToAction("Index");

            Ramo ramo = new Ramo() { Productos= new List<Productos>() };
            if (!string.IsNullOrEmpty(model.ramoId))
            {
                var ramos = ListaRamos(UserSesion(), model.ramoId, string.Empty);
                ramo = ramos.FirstOrDefault();
                model.ramoDescripcion = ramo.Descripcion;

                if (!string.IsNullOrEmpty(model.productoId))
                {
                    ramo.Productos = ramo.Productos.Where(w => w.id == model.productoId).ToList();
                    model.productoDescripcion = ramo.Productos.FirstOrDefault().Descripcion;
                }
                else
                {
                    ramo.Productos = new List<Productos>();
                }

            }

            return View(model);
        }
        public ActionResult Ramo()
        {
            ModelRamos mramo = new ModelRamos();
            mramo.Lista = ListaRamos(UserSesion(),string.Empty,string.Empty);

            return View(mramo);
        }

        [HttpPost]
        public ActionResult Ramo(ModelRamos model)
        {

            model.Lista = ListaRamos(UserSesion(), model.id, model.Descripcion);

            return View(model);
        }

        [HttpGet]
        public ActionResult SelectRamo(string id)
        {
            return RedirectToAction("Main",new { ramoid = id });
        }

        [HttpGet]
        public ActionResult SelectProducto(string id, string ramoid)
        {
            var param = new { ramoid= ramoid, productoid = id };

            return RedirectToAction("Main", param);
        }
        public ActionResult Producto(string ramoid)
        {
            ModelProductos producto = new ModelProductos();
            producto.Lista= ListaProductos(UserSesion(), ramoid,string.Empty, string.Empty);

            return View(producto);
        }

        [HttpPost]
        public ActionResult Producto(string ramoid, ModelProductos model)
        {
            model.Lista = ListaProductos(UserSesion(), ramoid, model.id, model.Descripcion);

            return View(model);
        }

        public JsonResult SearchRamoId(string ramoid)
        {
            Ramo ramo = new Ramo();
            if (!string.IsNullOrEmpty(ramoid))
            {
                var ramos = ListaRamos(UserSesion(), ramoid, string.Empty);

                ramo = ramos.FirstOrDefault();
            }

            return Json(new { ramoid = ramo.id, ramoDescripcion = ramo.Descripcion }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchProductoId(string ramoid, string id)
        {
            Ramo ramo = new Ramo();
            string productodescripcion=string.Empty;

            if (!string.IsNullOrEmpty(ramoid))
            {
                var ramos = ListaRamos(UserSesion(), ramoid, string.Empty);

                ramo = ramos.FirstOrDefault();

                if (!string.IsNullOrEmpty(id))
                {
                    ramo.Productos = ramo.Productos.Where(w => w.id == id).ToList();
                }
                else
                {
                    ramo.Productos = new List<Productos>();
                }

            }
            else if (!string.IsNullOrEmpty(id))
            {
                var products = ListaProductos(UserSesion(), string.Empty, id, string.Empty);

                if (products.Any())
                {
                    var product = products.FirstOrDefault();

                    var ramos = ListaRamos(UserSesion(), product.Ramo.id, string.Empty);

                    ramo = ramos.FirstOrDefault();


                    ramo.Productos = ramo.Productos.Where(w => w.id == id).ToList();
                    

                }

            }

            if (ramo.Productos.Any())
            {
                productodescripcion = ramo.Productos.FirstOrDefault().Descripcion;
            }

            return Json(new { ramoid = ramo.id, ramoDescripcion = ramo.Descripcion, productoDescripcion = productodescripcion }, JsonRequestBehavior.AllowGet);
        }

        private bool IsUserValid(ModelUser usuario)
        {
            var isValid = usuario != null;

            if (!isValid)
            {
                ViewBag.Msg = "Debe capturar información válida";
            }
            return isValid;
        }

        public Usuario Login(ModelUser usuario)
        {
            Usuario result = null;
            try
            {
                HttpContent inputContent;
                string inputJson = "";
                //inputJson = JsonConvert.SerializeObject(param);
                inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");

                HttpClient _client = new HttpClient();
                HttpResponseMessage response;
                _client = new HttpClient();
                

                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token.Replace("\"", ""));
                response = _client.PostAsync(uri + "Usuarios?usuario=" + usuario.UserName + "&password=" + usuario.Pws, inputContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<Usuario>(response.Content.ReadAsStringAsync().Result);
                }
                

            }
            catch (Exception ex)
            {
                throw;
            }

            return result; ;
        }

        public List<Ramo> ListaRamos(Usuario usuario, string idRamo, string descripcion)
        {
            List<Ramo> result = null;
            try
            {
                
                HttpContent inputContent;
                string inputJson = "";
                //inputJson = JsonConvert.SerializeObject(param);
                inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");

                HttpClient _client = new HttpClient();
                HttpResponseMessage response;
                _client = new HttpClient();


                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token.Replace("\"", ""));
                response = _client.GetAsync(uri + "GetRamosSearch?pais=" + usuario.Pais + "&id=" + idRamo + "&descripcion=" + descripcion).Result;

                if (response.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<List<Ramo>>(response.Content.ReadAsStringAsync().Result);
                }


            }
            catch (Exception ex)
            {
                throw;
            }

            return result; ;
        }

        public List<Productos> ListaProductos(Usuario usuario, string idRamo, string idProducto, string descripcion)
        {
            List<Productos> result = null;
            try
            {

                HttpContent inputContent;
                string inputJson = "";
                //inputJson = JsonConvert.SerializeObject(param);
                inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");

                HttpClient _client = new HttpClient();
                HttpResponseMessage response;
                _client = new HttpClient();


                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.Token.Replace("\"", ""));
                response = _client.GetAsync(uri + "GetProductosSearch?pais=" + usuario.Pais + "&idRamo=" + idRamo + "&id=" + idProducto + "&descripcion=" + descripcion).Result;

                if (response.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<List<Productos>>(response.Content.ReadAsStringAsync().Result);
                }


            }
            catch (Exception ex)
            {
                throw;
            }

            return result; ;
        }

        public Usuario UserSesion()
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            return usuario;
        }

    }
}