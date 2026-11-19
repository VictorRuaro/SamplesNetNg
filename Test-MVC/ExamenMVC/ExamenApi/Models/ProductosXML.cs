using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ExamenApi.Models
{

    [Serializable()]
    public class ProductosXML
    {
        [XmlAttribute("id")]
        public string id { get; set; }

        [XmlAttribute("Pais")]
        public string Pais { get; set; }

        [XmlAttribute("Descripcion")]
        public string Descripcion { get; set; }
    }

    [Serializable()]
    public class RamoXML
    {
        [XmlAttribute("id")]
        public string id { get; set; }

        [XmlAttribute("Pais")]
        public string Pais { get; set; }

        [XmlAttribute("Descripcion")]
        public string Descripcion { get; set; }

        [XmlElement("Productos")]
        public ProductosXML[] Productos { get; set; }
    }

    [Serializable()]
    [XmlRootAttribute("RiesgosLATAM")]
    public class RiesgosLATAM
    {
        [XmlElement("Ramo")]
        public RamoXML[] Ramo { get; set; }
    }
}