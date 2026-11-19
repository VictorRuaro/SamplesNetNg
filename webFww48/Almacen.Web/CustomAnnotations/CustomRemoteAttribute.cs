using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web.CustomAnnotations
{
    public class CustomRemoteAttribute : RemoteAttribute, IClientValidatable
    {
        public CustomRemoteAttribute(string routeName)
            : base(routeName)
        {
        }

        public CustomRemoteAttribute(string action, string controller)
            : base(action, controller)
        {
        }

        public CustomRemoteAttribute(string action, string controller, string areaName) 
            : base(action, controller, areaName)
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            object obj = value;

            if (!string.IsNullOrEmpty(AdditionalFields))
            {
                //List<string> fields = AdditionalFields.Split(',').ToList();

                //foreach (var item in fields)
                //{
                //    PropertyInfo otherProperty = validationContext.ObjectType.GetProperty(item);
                //    if (otherProperty == null)
                //    {
                //        return new ValidationResult(
                //            string.Format(CultureInfo.CurrentCulture, "No se pudo encontrar una propiedad con nombre '{0}'.", item));
                //    }

                //    object otherValue = otherProperty.GetValue(validationContext.ObjectInstance);
                //    dic.Add(item, otherValue);
                //}

                obj = validationContext.ObjectInstance;// FromDictToAnonymousObj("SolicitudDetalleVM", dic);

            }



            // Get the controller using reflection  
            Type controller = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(type => type.Name.ToLower() == string.Format("{0}Controller",
                    this.RouteData["controller"].ToString()).ToLower());
            if (controller != null)
            {
                // Get the action method that has validation logic  
                MethodInfo action = controller.GetMethods()
                    .FirstOrDefault(method => method.Name.ToLower() ==
                        this.RouteData["action"].ToString().ToLower());
                if (action != null)
                {
                    // Create an instance of the controller class  
                    object instance = Activator.CreateInstance(controller);
                    // Invoke the action method that has validation logic  
                    object response = action.Invoke(instance, new object[] { obj });
                    if (response is JsonResult)
                    {
                        object jsonData = ((JsonResult)response).Data;
                        if (jsonData is bool)
                        {
                            return (bool)jsonData ? ValidationResult.Success :
                                new ValidationResult(this.ErrorMessage);
                        }
                    }
                }
            }
            return new ValidationResult(base.ErrorMessageString);
        }

        public static object FromDictToAnonymousObj<TValue>(string NameClass,IDictionary<string, TValue> dict)
        {

            Type objectType = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(type => type.Name.ToLower() == NameClass.ToLower());

            object instance = Activator.CreateInstance(objectType);

            foreach (var item in dict)
            {
                PropertyInfo piShared = objectType.GetProperty(item.Key);
                piShared.SetValue(instance, item.Value);
            }

            return instance;

        }
    }
}