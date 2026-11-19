using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamenMVC.Models
{
    public class ModelUser
    {

        [Display(Description = "Usuario")]
        public string UserName { get; set; }

        [Display(Description = "Password")]
        public string Pws { get; set; }

    }
}