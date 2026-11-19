using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacen.Web.CustomAnnotations;

namespace Almacen.Web.ValidatorProvider
{
    public static class AdapterConfig
    {
        public static void RegisterGlobalValidatorProvider()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredIfAttribute), typeof(RequiredIfAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ExistenciasAttribute), typeof(ExistenciasAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ValueComparisonAttribute), typeof(ValueComparisonAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DateTimeCompareAttribute), typeof(DateTimeCompareAdapter));
        }
    }
}