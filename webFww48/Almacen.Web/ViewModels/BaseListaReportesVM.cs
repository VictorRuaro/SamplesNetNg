using Almacen.Infraestructura;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class BaseListaReportesVM<T> where T:class
    {
        private string _titulo;
        public BaseListaReportesVM()
        {
            Paginacion = new Paginacion<T>();
            Paginador = new PaginadorViewModels();
        }

        public virtual string Titulo
        {
            get {

                if (string.IsNullOrEmpty(_titulo))
                {
                    _titulo = typeof(T).Name;
                    string nombre = _titulo.Replace("VM", "");
                    MemberInfo property = typeof(T).GetProperty(nombre);
                    if (property != null)
                    {
                        var dd = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

                        if (dd != null)
                        {
                            _titulo = dd.Name;
                        }
                    }
                    else
                    {
                        MemberInfo propertyDes = typeof(T).GetProperty("Descripcion");
                        if (propertyDes != null)
                        {
                            var ddes = propertyDes.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

                            if (ddes != null)
                            {
                                _titulo = ddes.Name;
                            }
                        }
                    }
                }

                return string.Format("Catálogos - {0}", _titulo);
            }
            set {
                _titulo =value ;
            }
        }

        public virtual Paginacion<T> Paginacion { get; set; }

        public virtual PaginadorViewModels Paginador { get; set; }

        public virtual T Filtros { get; set; }

        public bool SinDatosListado { get; set; }

    }

    
}