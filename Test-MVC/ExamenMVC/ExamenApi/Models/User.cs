using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ExamenApi.Models
{
    [Serializable()]
    public class User
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("UserName")]
        public string UserName { get; set; }
        [XmlAttribute("Pws")]
        public string Pws { get; set; }
        [XmlAttribute("Pais")]
        public string Pais { get; set; }
    }

    [Serializable()]
    public class UserPais
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        
        [XmlAttribute("Pais")]
        public string Pais { get; set; }

        [XmlAttribute("Descripcion")]
        public string Descripcion { get; set; }

        [XmlElement("User")]
        public User[] Users { get; set; }
    }

    [Serializable()]
    [XmlRootAttribute("UsuariosLATAM")]
    public class UsuariosLATAM
    {
        [XmlElement("UserPais")]
        public UserPais[] UserPais { get; set; }
    }
}