
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web.CustomAnnotations
{
    public class RequiredIfAttributeAdapter : DataAnnotationsModelValidator<RequiredIfAttribute>
    {
        public RequiredIfAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredIfAttribute attribute)
        : base(metadata, context, attribute)
        { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRule { ErrorMessage = "Requerido", ValidationType = "requiredif" } };
        }

    }
}