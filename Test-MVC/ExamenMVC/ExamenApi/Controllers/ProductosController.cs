using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Xml.Serialization;
using Comun;
using ExamenApi.Models;

namespace ExamenApi.Controllers
{
    public class ProductosController : ApiController
    {
        RiesgosLATAM usuarios = new RiesgosLATAM();
        public ProductosController()
        {
            usuarios = DatosProductos();
        }

        private RiesgosLATAM DatosProductos()
        {
            RiesgosLATAM result = new RiesgosLATAM();

            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var path = dir + "DataXML\\Productos_LATAM.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(RiesgosLATAM));

            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                result = (RiesgosLATAM)serializer.Deserialize(stream);
            };

            return result;
        }

        [HttpGet, Route("api/GetRamos")]
        public List<Ramo> GetRamos(string pais)
        {
            List<Ramo> result = null;

            result = usuarios.Ramo.Where(w=> w.Pais== pais)
                .Select(s => new Ramo()
                {
                    id = s.id,
                    Descripcion = s.Descripcion,
                    Pais = s.Pais,
                    Productos = s.Productos.Select(sp => new Productos { id = sp.id, Descripcion = sp.Descripcion, Pais = sp.Pais }).ToList()
                }
                ).ToList();

            return result;
        }

        [HttpGet, Route("api/GetRamosSearch")]
        [AllowAnonymous]
        public List<Ramo> GetRamosSearch(string id, string descripcion, string pais)
        {
            List<Ramo> result = null;

            var rex = usuarios.Ramo.Where(w => w.Pais == pais).Select(s => new Ramo()
                 {
                     id = s.id,
                     Descripcion = s.Descripcion,
                     Pais = s.Pais,
                     Productos = s.Productos.Select(sp => new Productos { id = sp.id, Descripcion = sp.Descripcion, Pais = sp.Pais }).ToList()
                 }
                ).ToList();

            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(descripcion))
            {
                result = rex;
            }
            else if(string.IsNullOrEmpty(id))
            {
                result = result = rex.Where(w => w.Descripcion.Contains(descripcion.ToUpper())).ToList();
            }else if (string.IsNullOrEmpty(descripcion))
            {
                result = result = rex.Where(w => w.id == id).ToList();
            }
            else
            {
                result = result = rex.Where(w => w.id == id && w.Descripcion.Contains(descripcion.ToUpper())).ToList();
            }

            return result;
        }

        [HttpGet, Route("api/GetProductosSearch")]
        [AllowAnonymous]
        public List<Productos> GetProductosSearch(string id, string descripcion, string idRamo, string pais)
        {
            List<Productos> result = null;

            
                var rex = usuarios.Ramo.Where(w => w.Pais == pais)
                    .Select(s => new Ramo()
                    {
                        id = s.id,
                        Descripcion = s.Descripcion,
                        Pais = s.Pais,
                        Productos = s.Productos.Select(sp => new Productos { id = sp.id, Descripcion = sp.Descripcion, Pais = sp.Pais, 
                            Ramo = new Ramo() {
                                id = s.id,
                                Descripcion = s.Descripcion,
                                Pais = s.Pais
                            } 
                        }).ToList()
                    }
                ).ToList();

            if (string.IsNullOrEmpty(idRamo))
            {
                result = rex.SelectMany(s => s.Productos).ToList();
            }
            else
            {
                result = rex.Where(w => w.id == idRamo).SelectMany(s => s.Productos).ToList();
            }

            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(descripcion))
            {
                result = result.ToList();
            }
            else if(string.IsNullOrEmpty(id))
            {
                result = result.Where(w=> w.Descripcion.Contains(descripcion.ToUpper())).ToList();
            }
            else if (string.IsNullOrEmpty(descripcion))
            {
                result = result.Where(w => w.id == id).ToList();
            }
            else
            {
                result = result.Where(w => w.id == id && w.Descripcion.Contains(descripcion.ToUpper())).ToList();
            }

            return result;
        }

    }
}
