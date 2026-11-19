using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web.Tools
{
    public static class CustomExtension
    {

        public static async Task<SelectList> convertSelectListAsyc<T>(List<T> Lista, string nameFieldValue, string nameFieldText, bool optionSelect = false, object SelectectValue = null)
        {
            return await createSelectList(Lista, nameFieldValue, nameFieldText, optionSelect, SelectectValue);

        }

        private static Task<SelectList> createSelectList<T>(List<T> Lista, string nameFieldValue, string nameFieldText, bool optionSelect=false, object SelectectValue = null) 
        {
            SelectList result = new SelectList(new List<T>());

            List<SelectListItem> items = new SelectList(Lista, nameFieldValue, nameFieldText).ToList();

            if (optionSelect)
            {
                items.Insert(0, (new SelectListItem { Text = "--Seleccione--", Value = "" }));
            }

            result = new SelectList(items, "Value", "Text", SelectectValue);

            return Task.FromResult(result);
        }

        public static SelectList convertSelectList<T>(this HtmlHelper helper, List<T> Lista, string nameFieldValue, string nameFieldText, bool optionSelect = false, object SelectectValue = null)
        {
            SelectList result = new SelectList(new List<T>());
            result = convertSelectList(Lista, nameFieldValue, nameFieldText, optionSelect, SelectectValue);

            return result;
        }

        public static SelectList convertSelectList<T>(List<T> Lista, string nameFieldValue, string nameFieldText, bool optionSelect = false, object SelectectValue=null)
        {
            SelectList result;
            List<SelectListItem> items = new SelectList(Lista, nameFieldValue, nameFieldText).ToList();

            if (optionSelect)
            {
                items.Insert(0, (new SelectListItem { Text = "--Seleccione--", Value = "" }));
            }
            else
            {
                items.Insert(0, (new SelectListItem { Text = "", Value = "" }));
            }

            result = new SelectList(items, "Value", "Text", SelectectValue);
            return result;
        }

        public static string getParameterUrl(this HtmlHelper helper, string Url, string Key)
        {
            System.Collections.Specialized.NameValueCollection coll = System.Web.HttpUtility.ParseQueryString(Url.Split('?')[1]);

            string param = "";
            var qvar = coll.AllKeys.Where(w => w == Key).FirstOrDefault();

            param = coll[qvar];

            return param;
        }

        public static string ToTitleCase(this HtmlHelper helper, string str)
        {
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string ToTitleCase(string str)
        {
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string StreamImgToBase64(Stream imgStream)
        {
            string resultImage = "";

            if (imgStream != null)
            {
                try
                {
                    Bitmap image;
                    MemoryStream m;

                    image = new Bitmap(imgStream);
                    m = new MemoryStream();
                    image.Save(m, image.RawFormat);

                    if (m != null)
                    {
                        resultImage = "data:image/png;base64," + Convert.ToBase64String(m.ToArray(), 0, m.ToArray().Length);
                    }

                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }

            return resultImage;
        }

        public static byte[] ConvertHttpFileToByte(HttpPostedFileBase Archivo)
        {

            byte[] file = new byte[Archivo.ContentLength];
            Archivo.InputStream.Read(file, 0, file.Length);

            return file;

        }

        public static string GetFileName(string fileName)
        {
            string result = "";

            FileInfo file = new FileInfo(fileName);

            result=file.Name;

            return result;
        }

        public static string ToMultiple(this HtmlHelper helper, object parametroNumero, int multiplo)
        { 
            int result = 0;

            result = multiplo;

            int Numero = int.Parse(parametroNumero.ToString());

            if (Numero > multiplo)
            {
                double div = Numero / multiplo;

                int partEntera = (int)Math.Round(div, 0);

                result = partEntera * multiplo;

            }

            return result.ToString();
        }

        public static string ToMultiple(object parametroNumero, int multiplo)
        {
            int result = 0;

            result = multiplo;

            int Numero = int.Parse(parametroNumero.ToString());

            if (Numero > multiplo)
            {
                double div = Numero / multiplo;

                int partEntera = (int)Math.Round(div, 0);

                result = partEntera * multiplo;

            }

            return result.ToString();
        }
    }
}