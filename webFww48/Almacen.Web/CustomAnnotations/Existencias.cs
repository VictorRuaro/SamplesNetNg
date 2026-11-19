using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Routing;

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Almacen.Web.CustomAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ExistenciasAttribute : ValidationAttribute, IClientValidatable
    {

        public RouteValueDictionary RouteData { get; private set; }

        public string AdditionalFields { get; private set; }

        public object Cantidad { get; private set; }

        public object idAlmacen { get; private set; }

        public object idCategoria { get; private set; }

        public override bool RequiresValidationContext
        {
            get { return true; }
        }

        #region Constructor

        public ExistenciasAttribute(string routeName, string AdditionalFields)
            : base("'{0}' se requiere porque '{1}' tiene un valor {3}'{2}'.")
        {
            this.AdditionalFields = AdditionalFields;
            if (this.RouteData == null)
            {
                this.RouteData = new RouteValueDictionary();
            }
            this.RouteData.Add("route", routeName);
        }

        public ExistenciasAttribute(string action, string controller, string AdditionalFields)
            : base("'{0}' se requiere porque '{1}' tiene un valor {3}'{2}'.")
        {
            this.AdditionalFields = AdditionalFields;
            if(this.RouteData==null)
            {
                this.RouteData = new RouteValueDictionary();
            }
            this.RouteData.Add("action", action);
            this.RouteData.Add("controller", controller);
        }

        public ExistenciasAttribute(string action, string controller, string areaName, string AdditionalFields) 
            : base("'{0}' se requiere porque '{1}' tiene un valor {3}'{2}'.")
        {
            this.AdditionalFields = AdditionalFields;
            if (this.RouteData == null)
            {
                this.RouteData = new RouteValueDictionary();
            }
            this.RouteData.Add("action", action);
            this.RouteData.Add("controller", controller);
            this.RouteData.Add("area", areaName);

        }

        #endregion

        public override string FormatErrorMessage(string name)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                base.ErrorMessageString,
                name,
                this.AdditionalFields);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }

            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //var serialize = new System.Web.Script.Serialization.JavaScriptSerializer();

            //object obj = value;


            //dic.Add(validationContext.MemberName, value);


            //if (!string.IsNullOrEmpty(AdditionalFields))
            //{
            //    List<string> fields = AdditionalFields.Split(',').ToList();

            //    foreach (var item in fields)
            //    {
            //        PropertyInfo otherProperty = validationContext.ObjectType.GetProperty(item);
            //        if (otherProperty == null)
            //        {
            //            return new ValidationResult(
            //                string.Format(CultureInfo.CurrentCulture, "No se pudo encontrar una propiedad con nombre '{0}'.", item));
            //        }

            //        object otherValue = otherProperty.GetValue(validationContext.ObjectInstance);
            //        dic.Add(item, otherValue);


            //    }

            //    //obj = validationContext.ObjectInstance;

            //}

            //Type tipo = validationContext.ObjectType;


            //var sObject = dic.ToObject(validationContext.ObjectInstance);

            PropertyInfo otheridSustanciaMaterial = validationContext.ObjectType.GetProperty("idAlmacen");
            if (otheridSustanciaMaterial == null)
            {
                return new ValidationResult(
                    string.Format(CultureInfo.CurrentCulture, "No se pudo encontrar una propiedad con nombre '{0}'.", "idSustanciaMaterial"));
            }

            this.idAlmacen = otheridSustanciaMaterial.GetValue(validationContext.ObjectInstance);

            PropertyInfo otheridCategoria = validationContext.ObjectType.GetProperty("idCategoria");
            if (otheridCategoria == null)
            {
                return new ValidationResult(
                    string.Format(CultureInfo.CurrentCulture, "No se pudo encontrar una propiedad con nombre '{0}'.", "idCategoria"));
            }

            this.idCategoria = otheridCategoria.GetValue(validationContext.ObjectInstance);

            this.Cantidad = value;


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
                    object response = action.Invoke(instance, new object[] { Cantidad, idAlmacen, idCategoria });
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

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule rule = new ModelClientValidationRule
                {
                    ErrorMessage = ErrorMessageString,
                    ValidationType = "existencias"
                };


            Dictionary<string, object> dic = new Dictionary<string, object>();
            var serialize = new System.Web.Script.Serialization.JavaScriptSerializer();
            

            if (!string.IsNullOrEmpty(AdditionalFields))
            {
                List<string> fields = AdditionalFields.Split(',').ToList();

                foreach (var item in fields)
                {

                    PropertyInfo otherProperty = metadata.Container.GetType().GetProperty(item);
                    object otherValue = null;
                    if (otherProperty != null)
                    {
                        otherValue = otherProperty.GetValue(metadata.Container);
                        dic.Add(item, otherValue);
                    }

                }


                //string model = serialize.Serialize(metadata.Container);

                //rule.ValidationParameters["object"] = model;// FromDictToAnonymousObj("SolicitudDetalleVM", dic);

                //var obj = dic.ToObject(metadata.Container);

                rule.ValidationParameters["additionalfields"] = serialize.Serialize(dic);

            }



            rule.ValidationParameters["parametername"] = "Cantidad";

            rule.ValidationParameters["parametervalue"] = metadata.Model is bool ? metadata.Model.ToString().ToLower() : metadata.Model;

            rule.ValidationParameters["url"] = GetUrl(context);

            //// Add the input name the remote validation function expects.  If no
            //// generic input name is specified, use the name of the parameter to be validated.
            //rule.ValidationParameters["inputname"] =String.IsNullOrEmpty(Attribute.InputName) ? metadata.PropertyName : Attribute.InputName;


            yield return rule;
        }

        private string GetUrl(ControllerContext context)
        {

            string Controller = this.RouteData["controller"].ToString();

            string Action = this.RouteData["Action"].ToString();

            RouteValueDictionary rvd = new RouteValueDictionary() {
                { "controller", Controller },
                { "action", Action }
            };

            var virtualPath = RouteTable.Routes.GetVirtualPath(context.RequestContext, null, rvd);
            if (virtualPath == null)
            {
                throw new InvalidOperationException("No route matched!");
            }

            return virtualPath.VirtualPath;
        }

        public string replacechars(string value)
        {
            string result = "";

            char cr = (char)13;
            char lf = (char)10;
            char tab = (char)9;

            string msg = value;

            msg = msg.Replace(System.Environment.NewLine, "");

            msg = msg.Replace(cr.ToString(), "");
            msg = msg.Replace(lf.ToString(), "");
            msg = msg.Replace(tab.ToString(), "");

            msg = msg.Replace("\r\n|\n", "");
            msg = msg.Replace("\r\n?|\n?", "");

            result = msg;

            return result;

        }

    }
    internal static class ObjectExtensions
    {
        public static object ToObject( this IDictionary<string, object> source, object instance)
        {
            var someObjectType = instance.GetType();
            var someObject = Activator.CreateInstance(someObjectType);

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static T DictionaryToObject<T>(this IDictionary<string, object> dict) where T : new()
        {
            var t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                KeyValuePair<string, object> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

                // ...and change the type
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}