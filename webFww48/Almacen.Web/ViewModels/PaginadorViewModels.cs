using Almacen.Infraestructura;
using System.Collections.Generic;
using System.Web.Routing;

namespace Almacen.Web.ViewModels
{

    public class PaginadorViewModels : PagedBase
    {
        public RouteValueDictionary ValoresQueryString { get; set; }
    }
}