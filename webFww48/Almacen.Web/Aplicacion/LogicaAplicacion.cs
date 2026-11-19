using Almacen.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Almacen.Web.ViewModels.Catalogos;
using Almacen.Data;
using Almacen.Web.Applicacion;
using Almacen.Infraestructura;

namespace Almacen.Web.Aplicacion
{
    public class LogicaAplicacion
    {
        public static  RespuestaFlujo ValidaSolicitudFlujo(SolicitudVM oSolicitud)
        {
            RespuestaFlujo result = new RespuestaFlujo();

            result.ValueSuccess = false;
            result.Status = ValorRespuesta.NoCompleto;

            var User = HttpContext.Current.User;

            bool isAdmin = false;
            bool isAutorizador = false;

            //Usuario oUsuarioActual = BuscarUsuario(idUsuarioActual);
            int paso = 0;

            if (oSolicitud.cEstatusSolicitud != null)
            {
                paso = oSolicitud.cEstatusSolicitud.NumEstatus.Value;
            }

            if (User.IsInRole(EnumRol.Administrador))
            {
                isAdmin = true;
            }

            if (User.IsInRole(EnumRol.Autorizador_Material) || User.IsInRole(EnumRol.Autorizador_Sustancias))
            {
                isAutorizador = true;
            }

            switch (paso)
            {
                case (int)EnumEstatus.Borrador: //borrador

                    //result.oUsuario = oUsuarioActual;

                    result.ValueSuccess = true;
                    result.Status = ValorRespuesta.Completo;
                    result.visibleBtnGuardar = true;


                    //si es admin, agregar accion nueva no contemplada en este estatus 
                    if (isAdmin == true)
                    {
                        //result.visibleBtnRechazar = true;
                    }


                    break;
                case (int)EnumEstatus.SinEnviar: //"Sin Enviar"

                    // valida que la solicitud tenga Lista agregada
                    if (oSolicitud.Materiales.Any())
                    {
                        result.ValueSuccess = true;
                        result.Status = ValorRespuesta.Completo;
                        result.Mensaje = "Solicitud completa y lista para enviar a autorizar";
                        result.visibleBtnEnviar = true;
                        result.tipoMensaje = "has-success";
                    }
                    else
                    {
                        result.ValueSuccess = true;
                        result.Status = ValorRespuesta.NoCompleto;
                        result.Mensaje = "La solicitud esta incompleta falta agregar 'Listado'";
                        result.tipoMensaje = "has-error";
                    }

                    //si no es admin, valida que el usuario que la genero sea el mismo de la sesion actual
                    if (isAdmin == true)
                    {
                        result.visibleBtnEditar = true;
                        result.visibleBtnEliminar = true;
                        result.visibleBtnGuardar = true;
                    }
                    else if (oSolicitud.UG == User.Identity.GetUserId())
                    {
                        result.visibleBtnEditar = true;
                        result.visibleBtnEliminar = true;
                        result.visibleBtnGuardar = true;
                    }

                    break;

                case (int)EnumEstatus.PorAutorizar: //"Por autorizar"

                    //si no es admin, valida que si el usuario de la sesion actual no es que la genero sea el que autorize las solicitudes de la instalacion 
                    if (isAdmin == true)
                    {
                        result.ValueSuccess = true;
                        result.Status = ValorRespuesta.Completo;
                        result.visibleBtnAutorizar = true;
                        result.visibleBtnRegresarSinEnviar = true;

                    }
                    else if (isAutorizador)
                    {
                        result.ValueSuccess = true;
                        result.Status = ValorRespuesta.Completo;
                        
                        if(oSolicitud.UG== User.Identity.GetUserId())
                        {
                            result.Mensaje = "La solicitud fue elaborada por el mismo Autorizador, solamente podra ser autorizada por otro Autorizador o Administrador";
                            result.visibleBtnAutorizar = false;
                        }
                        else
                        {
                            result.visibleBtnAutorizar = true;
                        }

                        result.visibleBtnRegresarSinEnviar = true;

                    }
                    else
                    {
                        result.ValueSuccess = true;
                        result.Status = ValorRespuesta.NoCompleto;
                    }

                    break;

                case (int)EnumEstatus.Pendiente: //"Autorizada"

                    var validaExistencia = verificaExistenciaPorLote(oSolicitud);

                    result.ValueSuccess = true;
                    result.Status = ValorRespuesta.NoCompleto;
                    result.visibleBtnDescarga = true;
                    result.visibleBtnGenerarSalida=true;

                    //si es admin, agregar accion nueva no contemplada en este estatus 
                    if (isAdmin == true)
                    {
                        result.visibleBtnRegresarSinEnviar = true;
                    }

                    break;

            }

            return result;
        

    }

        private static bool verificaExistenciaPorLote(SolicitudVM solicitud)
        {
            bool result = false;

            var Listaid = solicitud.Materiales.Select(s => s.idSustanciaMaterial).ToList();

            //List<ExistenciaAlmacen> ListaAlmacen = ListaExistencias(solicitud.idCategoria, Listaid);

            

            foreach (SolicitudDetalleVM item in solicitud.Materiales)
            {
                
                item.idSolicitudDetalleEstatus = (int)EnumDetalleEstatus.NoAlcanzaExistencia;

                //Mostrar un apartado con los datos de existencias para seleccionar el lote en caso de no cubrir la cantidad solicitada
                //Permitir la salida de los materiales con la regla de primeras entradas primeras salidas.

                List<ExistenciaAlmacen> ListaAlmacen = ObtenerExistenciasPresentacion(solicitud.idCategoria, item.idAlmacen.Value);

                ExistenciaAlmacen almacen = ListaAlmacen.Where(w => w.idSustanciaMaterial == item.idSustanciaMaterial).OrderBy(o => o.FechaRecepcion).FirstOrDefault();

                if (almacen != null)
                {
                    if (ValidaExistencia(item.CantidadAutorizada, almacen.Existencia))
                    {

                        item.CantidadSurtida = item.CantidadAutorizada;
                        item.idAlmacen = almacen.idAlmacen;
                        item.Existencia = almacen.Existencia;
                        item.TotalSurtida = item.CantidadSurtida;

                        item.idSolicitudDetalleEstatus = (int)EnumDetalleEstatus.AlcanzaExistencia;

                        SolicitudDetalleLoteVM lote = new SolicitudDetalleLoteVM();
                        
                        lote.idAlmacen = almacen.idAlmacen;
                        lote.idSolicitudDetalle = item.idSolicitudDetalle;
                        lote.CantidadSurtida = item.CantidadSurtida;
                        lote.Existencia = almacen.Existencia;

                        item.tSolicitudDetalleLote.Add(lote);

                    }
                }else
                {
                    item.CantidadSurtida = 0;
                    item.Existencia = 0;
                    item.TotalSurtida = 0;

                    item.idSolicitudDetalleEstatus = (int)EnumDetalleEstatus.NoAlcanzaExistencia;
                }

            }



            return result;
        }

        public static List<ExistenciaAlmacen> ObtenerExistenciasPresentacion(int idCategoria, int idAlmacen)
        {

            List<ExistenciaAlmacen> lista = new List<ExistenciaAlmacen>();

            if (idCategoria == (int)EnumCategoria.Sustancias)
            {
                using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
                {
                    tAlmacenSustancia Sustancia = new tAlmacenSustancia();

                    Sustancia = new RepositoryGenerico<tAlmacenSustancia>(db)
                        .EncontrarPor(w => w.ST == true
                        && w.Existencia > 0
                        && w.idAlmacen == idAlmacen
                        ).FirstOrDefault();

                    if (Sustancia != null)
                    {
                        lista = new RepositoryGenerico<tAlmacenSustancia>(db)
                                .EncontrarPor(w => w.ST == true
                                && w.Existencia > 0
                                && w.idSustanciaMaterial == Sustancia.idSustanciaMaterial
                                && w.idTipoEnvase == Sustancia.idTipoEnvase
                                && w.idUMCantidadEnvase == Sustancia.idUMCantidadEnvase
                                && w.CantidadPorEnvase == Sustancia.CantidadPorEnvase
                                )
                                .Select(s => new ExistenciaAlmacen()
                                {
                                    idAlmacen = s.idAlmacen,
                                    idSustanciaMaterial = s.idSustanciaMaterial,
                                    FechaRecepcion = s.FechaRecepcion,
                                    Lote = s.NumDeLoteInterno,
                                    Existencia = s.Existencia
                                }
                                ).ToList();
                    }

                }
            }
            else
            {

                using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
                {
                    tAlmacenMaterial material = new tAlmacenMaterial();

                    material = new RepositoryGenerico<tAlmacenMaterial>(db)
                        .EncontrarPor(w => w.ST == true
                        && w.Existencia > 0
                        && w.idAlmacen == idAlmacen
                        ).FirstOrDefault();

                    if (material != null)
                    {
                        lista = new RepositoryGenerico<tAlmacenMaterial>(db)
                            .EncontrarPor(w => w.ST == true
                            && w.Existencia > 0
                            && w.idSustanciaMaterial == material.idSustanciaMaterial
                            && w.idPresentacion == material.idPresentacion
                            )
                            .Select(s => new ExistenciaAlmacen()
                            {
                                idAlmacen = s.idAlmacen,
                                idSustanciaMaterial = s.idSustanciaMaterial,
                                FechaRecepcion = s.FechaRecepcion,
                                Lote = s.NumDeLoteInterno,
                                Existencia = s.Existencia
                            }
                            ).ToList();
                    }

                }
            }

            return lista;
        }


        public static List<PresentacionSustanciaVM> ObtenerPresentacionesSustancia(int idSustanciaMaterial)
        {
            List<PresentacionSustanciaVM> result = new List<PresentacionSustanciaVM>();
            List<AlmacenSustanciaVM> ListaAlmacenSustancia = new List<AlmacenSustanciaVM>();

            Alerta Alerta = new Alerta();
            AlertaXSustanciaMaterialVM Alertadetalle = new AlertaXSustanciaMaterialVM();

            DateTime fechacaducidad = DateTime.Now;
            //int dias = 0;

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
               
                //Alerta = new LeerCatalogosServicio<Alerta, tAlerta>(db)
                //    .EncontrarPor(
                //    w => w.ST == true
                //    && w.idAlertaUnidad == (int)EnumAlertaUnidad.Dias
                //    ).FirstOrDefault();

                //if (Alerta != null)
                //{
                //    dias = int.Parse(Alerta.Valor);

                //    Alertadetalle = new LeerCatalogosServicio<AlertaXSustanciaMaterialVM, tAlertaXSustanciaMaterial>(db)
                //        .EncontrarPor(
                //        w => w.ST == true
                //        && w.idSustanciaMaterial == idSustanciaMaterial
                //        && w.idAlerta == Alerta.idAlerta
                //        ).FirstOrDefault();
                //    if (Alertadetalle != null)
                //    {
                //        dias = Alertadetalle.Valor;
                //    }
                //}


                //fechacaducidad.AddDays(dias);

                ListaAlmacenSustancia = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>(db)
                    .EncontrarPor(
                    w => w.ST == true
                    && w.idSustanciaMaterial == idSustanciaMaterial
                    && w.Existencia > 0
                    && w.FechaCaducidad > fechacaducidad
                    );

            }

            result = ListaAlmacenSustancia
                .GroupBy(g =>
                new {
                    idSustanciaMaterial = g.idSustanciaMaterial,
                    idTipoEnvase = g.idTipoEnvase,
                    idUMCantidadEnvase = g.idUMCantidadEnvase,
                    CantidadPorEnvase = g.CantidadPorEnvase
                })
                .Select(s => new PresentacionSustanciaVM()
                {
                    idSustanciaMaterial = s.Key.idSustanciaMaterial,
                    idTipoEnvase = s.Key.idTipoEnvase,
                    idUMCantidadEnvase = s.Key.idUMCantidadEnvase,
                    CantidadPorEnvase = s.Key.CantidadPorEnvase,
                    Existencia = s.Sum(sm => sm.Existencia),
                    idAlmacen = s.FirstOrDefault().idAlmacen,// solo indica uno de los muchos lotes de esta presentacion
                    cTipoEnvase = s.FirstOrDefault().cTipoEnvase,
                    cUMCantidadEnvase = s.FirstOrDefault().cUMCantidadEnvase
                }
                ).ToList();

            return result;
        }

        public static List<PresentacionMaterialVM> ObtenerPresentacionesMaterial(int idSustanciaMaterial)
        {
            List<PresentacionMaterialVM> result = new List<PresentacionMaterialVM>();

            List<AlmacenMaterialVM> ListaAlmacenSustancia = new List<AlmacenMaterialVM>();

            Alerta Alerta = new Alerta();
            AlertaXSustanciaMaterialVM Alertadetalle = new AlertaXSustanciaMaterialVM();

            DateTime fechacaducidad = DateTime.Now;
            //int dias = 0;

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                //Alerta = new LeerCatalogosServicio<Alerta, tAlerta>(db)
                //    .EncontrarPor(
                //    w => w.ST == true
                //    && w.idAlertaUnidad == (int)EnumAlertaUnidad.Porcentaje
                //    ).FirstOrDefault();

                //if (Alerta != null)
                //{
                //    dias = int.Parse(Alerta.Valor);

                //    Alertadetalle = new LeerCatalogosServicio<AlertaXSustanciaMaterialVM, tAlertaXSustanciaMaterial>(db)
                //        .EncontrarPor(
                //        w => w.ST == true
                //        && w.idSustanciaMaterial == idSustanciaMaterial
                //        && w.idAlerta == Alerta.idAlerta
                //        ).FirstOrDefault();
                //    if (Alertadetalle != null)
                //    {
                //        dias = Alertadetalle.Valor;
                //    }
                //}

                ListaAlmacenSustancia = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>(db)
                    .EncontrarPor(
                    w => w.ST == true
                    && w.idSustanciaMaterial == idSustanciaMaterial
                    && w.Existencia > 0
                    && w.FechaCaducidad > fechacaducidad
                    );

            }

            result = ListaAlmacenSustancia
                .GroupBy(g =>
                new {
                    idSustanciaMaterial = g.idSustanciaMaterial,
                    idTipoEnvase = g.idPresentacion,
                    idPresentacion = g.idPresentacion
                })
                .Select(s => new PresentacionMaterialVM()
                {
                    idSustanciaMaterial = s.Key.idSustanciaMaterial,
                    idTipoEnvase = s.Key.idTipoEnvase,
                    idPresentacion = s.Key.idPresentacion,
                    Existencia = s.Sum(sm => sm.Existencia),
                    idAlmacen = s.FirstOrDefault().idAlmacen,// solo indica uno de los muchos lotes de esta presentacion
                    cPresentacion = s.FirstOrDefault().cPresentacion

                }
                ).ToList();

            return result;
        }

        public static bool ValidaExistencia(int Cantidad, int existencia)
        {
            return existencia >= Cantidad;
        }


        public static List<ExistenciaAlmacen> ListaExistencias(int idCategoria, List<int> ListaSustanciaMaterial)
        {

            List<ExistenciaAlmacen> lista = new List<ExistenciaAlmacen>();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {

                lista = new RepositoryGenerico<tAlmacen>(db)
                    .EncontrarPor(w => w.ST == true && w.Existencia>0 
                    && ListaSustanciaMaterial.Contains(w.idSustanciaMaterial))
                    .Select(s => new ExistenciaAlmacen() {
                        idAlmacen =s.idAlmacen,
                        idSustanciaMaterial =s.idSustanciaMaterial,
                        FechaRecepcion = s.FechaRecepcion,
                        Lote =s.NumDeLoteInterno,
                        Existencia =s.Existencia }
                    ).ToList();

            }

            return lista;
        }

        public static int ExistenciasPorFechaLote(List<ExistenciaAlmacen> lista, int idSustanciaMaterial, bool sumarTodosLotes)
        {
            int existencia = 0;

            if (lista.Any())
            {
                if (sumarTodosLotes)
                {
                    existencia = lista.Where(w=> w.idSustanciaMaterial== idSustanciaMaterial).Select(s => s.Existencia).Sum();
                }
                else
                {
                    existencia = lista.Where(w => w.idSustanciaMaterial == idSustanciaMaterial).OrderBy(o => o.FechaRecepcion).Select(s => s.Existencia).FirstOrDefault();
                }
            }

            return existencia;
        }

        public static SolicitudVM EstableceEstatus(SolicitudVM oSolicitud, EnumEstatus? CambioEstatus=null)
        {
            //*** convertir a una sola con automapper para evitar duplicidad de funcionamiento

            tSolicitud tmp = AutoMapperGenericsHelper<SolicitudVM, tSolicitud>.ConvertToDBEntity(oSolicitud);

            tmp = EstableceEstatus(tmp, CambioEstatus);

            oSolicitud = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(tmp);

            return oSolicitud;
        }

        public static tSolicitud EstableceEstatus(tSolicitud oSolicitud, EnumEstatus? CambioEstatus = null)
        {

            List<EstatusSolicitudVM> estatus = null;

            var User = HttpContext.Current.User;

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                estatus = new LeerCatalogosServicio<EstatusSolicitudVM, cEstatusSolicitud>(db).ObtenerTodos();
            }

            if (CambioEstatus != null)
            {
                int Estatus = (int)CambioEstatus.Value;
                int idEstatus = estatus.Where(w => w.NumEstatus == Estatus).Select(s => s.idEstatusSolicitud).FirstOrDefault();

                oSolicitud.idEstatusSolicitud = idEstatus;
                //En el caso de que sea rechazada la solicitud debe estar sin autorizador
                if (idEstatus < (int)EnumEstatus.Pendiente && !string.IsNullOrEmpty(oSolicitud.idAutorizador))
                {
                    oSolicitud.idAutorizador = string.Empty;
                    oSolicitud.FechaAutoriza = null;
                }
            }
            else
            {
                int EstatusActual = estatus.Where(w => w.idEstatusSolicitud == oSolicitud.idEstatusSolicitud).Select(s => s.NumEstatus.Value).FirstOrDefault();
                int idEstatus = 0;

                //Flujo del cambio de estatus solicitud

                switch (EstatusActual)
                {
                    case (int)EnumEstatus.Borrador:

                        idEstatus = estatus.Where(w => w.NumEstatus == (int)EnumEstatus.PorAutorizar).Select(s => s.idEstatusSolicitud).FirstOrDefault();

                        oSolicitud.idEstatusSolicitud = idEstatus;
                        oSolicitud.FechaEnvio = DateTime.Now;
                        break;
                    case (int)EnumEstatus.SinEnviar:

                        idEstatus = estatus.Where(w => w.NumEstatus == (int)EnumEstatus.PorAutorizar).Select(s => s.idEstatusSolicitud).FirstOrDefault();

                        oSolicitud.idEstatusSolicitud = idEstatus;
                        oSolicitud.FechaEnvio = DateTime.Now;
                        break;
                    case (int)EnumEstatus.PorAutorizar:
                        idEstatus = estatus.Where(w => w.NumEstatus == (int)EnumEstatus.Pendiente).Select(s => s.idEstatusSolicitud).FirstOrDefault();

                        oSolicitud.idEstatusSolicitud = idEstatus;
                        oSolicitud.idAutorizador = User.Identity.GetUserId();
                        oSolicitud.FechaAutoriza = DateTime.Now;
                        break;
                    case (int)EnumEstatus.Pendiente:

                        bool MaterialesDevolucion = false;

                        MaterialesDevolucion = oSolicitud.tSolicitudDetalle.Where(w => w.CantidadDevolver > 0).Any();

                        if (MaterialesDevolucion)
                        {
                            idEstatus = estatus.Where(w => w.NumEstatus == (int)EnumEstatus.SurtidaDevolucion).Select(s => s.idEstatusSolicitud).FirstOrDefault();
                        }
                        else
                        {
                            idEstatus = estatus.Where(w => w.NumEstatus == (int)EnumEstatus.Surtida).Select(s => s.idEstatusSolicitud).FirstOrDefault();
                        }

                        oSolicitud.idEstatusSolicitud = idEstatus;
                        oSolicitud.idSurtidor= User.Identity.GetUserId();
                        oSolicitud.FechaSurtida = DateTime.Now;
                        break;
                    case (int)EnumEstatus.SurtidaDevolucion:
                        idEstatus = estatus.Where(w => w.NumEstatus == (int)EnumEstatus.Surtida).Select(s => s.idEstatusSolicitud).FirstOrDefault();

                        oSolicitud.idEstatusSolicitud = idEstatus;
                        oSolicitud.idUsuarioDevolucion= User.Identity.GetUserId();
                        oSolicitud.FechaDevolucion = DateTime.Now;
                        break;
                    //case (int)EnumEstatus.Surtida:
                    //no hay procesos para este estatus
                    //    break;
                }

            }

            

            return oSolicitud;
        }

        public static List<SolicitudDetalleVM> ListaSolicitudDetalle(List<SolicitudDetalleLoteVM> selecionados, List<SolicitudDetalleVM> sustancias)
        {
            List<SolicitudDetalleVM> result = new List<SolicitudDetalleVM>();


            
            foreach (var item in sustancias)
            {
                //&& !selecionados.Select(s => s.idAlmacen).Contains(w.idAlmacen)
                var lotes = selecionados.Where(w => w.idSustanciaMaterial == item.idSustanciaMaterial).ToList();

                if (lotes.Any())
                {
                    item.tSolicitudDetalleLote = lotes;


                    int cantidadSurtir = selecionados.Where(w => w.idSustanciaMaterial == item.idSustanciaMaterial)
                                                      .Sum(su => su.CantidadSurtida);

                    int cantidaddevoler = selecionados.Where(w => w.idSustanciaMaterial == item.idSustanciaMaterial)
                                                      .Sum(su => su.CantidadDevolver);

                    int existencia = selecionados.Where(w => w.idSustanciaMaterial == item.idSustanciaMaterial)
                                                      .Sum(su => su.Existencia);


                    item.CantidadSurtida = cantidadSurtir;
                    item.CantidadDevolver = cantidaddevoler;
                    item.Existencia = existencia;

                    item.TotalSurtida = cantidadSurtir - cantidaddevoler;

                    item.idAlmacen = selecionados.FirstOrDefault().idAlmacen;

                    bool AlcanzaExistencia = existencia >= cantidadSurtir;

                    if (AlcanzaExistencia)
                    {
                        item.idSolicitudDetalleEstatus = (int)EnumDetalleEstatus.AlcanzaExistencia;
                    }
                    else
                    {
                        item.idSolicitudDetalleEstatus = (int)EnumDetalleEstatus.NoAlcanzaExistencia;
                    }
                }
            }


            result = sustancias;

            return result;
        }
        public static List<SolicitudDetalleVM> ListaSolicitudDetalle_resp(List<ExistenciaAlmacen> selecionados, List<SolicitudDetalleVM> sustancias)
        {
            sustancias = sustancias
                          .GroupJoin(
                             selecionados,
                             r => r.idSustanciaMaterial,
                             rp => rp.idSustanciaMaterial,
                             (r, rp) => new
                             {
                                 d = r,
                                 rp = rp
                             }
                          )
                          .SelectMany(
                             temp0 => temp0.rp.DefaultIfEmpty(),
                             (temp0, s) =>
                             {
                                 if (s != null)
                                 {

                                     bool AlcanzaExistencia = s.Existencia >= temp0.d.CantidadAutorizada;
                                     temp0.d.CantidadSurtida = s.Existencia;
                                     temp0.d.Existencia = s.Existencia;
                                     temp0.d.idAlmacen = s.idAlmacen;

                                     if (AlcanzaExistencia)
                                     {
                                         temp0.d.idSolicitudDetalleEstatus = (int)EnumDetalleEstatus.AlcanzaExistencia;
                                     }
                                     else
                                     {
                                         temp0.d.idSolicitudDetalleEstatus = (int)EnumDetalleEstatus.NoAlcanzaExistencia;
                                     }

                                 }
                                 return temp0.d;
                             }
                      ).ToList();


            return sustancias;
        }
    }
    public class ExistenciaAlmacen
    {
        public int idAlmacen { get; set; }
        public int idSustanciaMaterial { get; set; }
        public int Lote { get; set; }

        public string LoteFormato { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public int Existencia { get; set; }
    }
}