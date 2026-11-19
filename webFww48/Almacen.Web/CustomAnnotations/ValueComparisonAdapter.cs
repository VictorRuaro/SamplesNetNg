
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web.CustomAnnotations
{
    public class ValueComparisonAdapter : DataAnnotationsModelValidator<ValueComparisonAttribute>
    {
        public ValueComparisonAdapter(ModelMetadata metadata, ControllerContext context, ValueComparisonAttribute attribute)
        : base(metadata, context, attribute)
        { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRule { ErrorMessage = "Requerido", ValidationType = "valuecomparison" } };
        }

    }
}