using Almacen.Data;
using System;

namespace Almacen.Web.Aplicacion
{
    public class CorreoServicio
    {
        public ResultCorreo EnviaCorreo(DatosCorreo datosCorreo)
        {
            ResultCorreo result = new ResultCorreo();
            try
            {
                using (CndAlmacenDopajeEntities db= new CndAlmacenDopajeEntities())
                {
                    int resp = db.spSendEmail("AlmacenDopaje",
                        datosCorreo.recipients,
                        datosCorreo.copy_recipients,
                        datosCorreo.body,
                        datosCorreo.body_format,
                        datosCorreo.subject);
                    if(resp > 0)
                    {
                        result.correoEnviado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "No se pudo enviar el correo: " + ex.Message;
                if (ex.InnerException!=null)
                {
                    msg += ex.InnerException.Message;
                }
                result.mensajeError = msg;
                
            }
            return result;
        }
    }

    public class DatosCorreo
    {
        public string recipients { get; set; }
        public string copy_recipients { get; set; }
        public string body { get; set; }
        public string body_format { get; set; }
        public string subject { get; set; }
        //public string file_attachments { get; set; }

    }

    public class ResultCorreo
    {
        public bool correoEnviado { get; set; }
        public string mensajeError { get; set; }
    }


}