using System.Text.RegularExpressions;

namespace Almacen.Infraestructura
{
    public class ResultSave
    {
        protected string _mensaje;
        public int Afectados { get; set; }
        public string Mensaje {
            get {
                return _mensaje;
            } set {
                //string pattern = @"\|\'|\r\n?|\n|" + "\"";
                //string replacement = "-";
                //Regex rgx = new Regex(pattern);
                //string result = rgx.Replace(value, replacement);
                _mensaje = value;
            }
        }
    }
}
