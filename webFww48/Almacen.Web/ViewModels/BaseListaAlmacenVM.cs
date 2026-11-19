using Almacen.Infraestructura;
using Almacen.Web.ViewModels.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class BaseListaAlmacenVM<TModel,TList> where TModel : new()
    {
        public BaseListaAlmacenVM()
        {
            Lista = new List<TList>();
            Material = new TModel();
        }

        public virtual string Titulo { get; set; }

        public virtual TModel Material { get; set; }
        public virtual List<TList> Lista { get; set; }

        public bool SinDatosListado { get; set; }

    }


}