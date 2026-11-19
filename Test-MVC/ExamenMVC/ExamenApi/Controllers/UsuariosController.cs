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
    public class UsuariosController : ApiController
    {

        UsuariosLATAM usuarios = new UsuariosLATAM();
        public UsuariosController()
        {
            usuarios = DatosUsuarios();
        }

        private UsuariosLATAM DatosUsuarios()
        {
            UsuariosLATAM result = new UsuariosLATAM();

            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var path = dir + "DataXML\\Usuarios_LATAM.xml";
            
            XmlSerializer serializer = new XmlSerializer(typeof(UsuariosLATAM));

            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                result = (UsuariosLATAM)serializer.Deserialize(stream);
            };

            return result;
        }

        // Post: Usuarios
        [HttpPost]
        [AllowAnonymous]
        public Usuario Login(string usuario, string password)
        {
            Usuario result = null;

            User user = usuarios.UserPais.SelectMany(s => s.Users).Where(w=> w.UserName== usuario && w.Pws== password).FirstOrDefault();

            if (user != null)
            {
                result = new Usuario()
                {
                    id = user.id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Pais = user.Pais,
                    Pws = user.Pws
                };
            }

            return result;
        }
    }
}