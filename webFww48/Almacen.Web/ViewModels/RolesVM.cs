using Almacen.Infraestructura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Almacen.Web.ViewModels
{
    public class RolesVM
    {
        public RolesVM()
        {
            //this.AspNetUsers = new HashSet<AspNetUsers>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        //public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
        
    }
}