
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web.CustomAnnotations
{
    public class ExistenciasAttributeAdapter : DataAnnotationsModelValidator<ExistenciasAttribute>
    {
        public ExistenciasAttributeAdapter(ModelMetadata metadata, ControllerContext context, ExistenciasAttribute attribute)
        : base(metadata, context, attribute)
        { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRule { ErrorMessage = "La cantidad debe ser menor", ValidationType = "existencias" } };
        }

    }
}