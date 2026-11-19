using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Almacen.Web.Aplicacion
{
    public class EnumsAplication
    {
    }

    public enum EnumCategoria
    {
        Sustancias = 1,
        Materiales = 2
    }

    public enum EnumEstatus
    {
        Borrador = 0,
        SinEnviar = 1,
        PorAutorizar = 2,
        Pendiente = 3,
        SurtidaDevolucion=4,
        Surtida = 5

    }

    public enum EnumDetalleEstatus
    {
        SinEstablecer = 0,
        NoAlcanzaExistencia = 1,
        AlcanzaExistencia = 2,
    }

    public sealed class EnumRol
    {

        private EnumRol() { }

        //public static readonly string Administrador = "Administrador";
        //public static readonly string Administrador_Almacen = "Administrador_Almacen";
        //public static readonly string Administrador_Informacion = "Administrador_Informacion";
        //public static readonly string Autorizador_Material = "Autorizador_Material";
        //public static readonly string Autorizador_Sustancias = "Autorizador_Sustancias";
        //public static readonly string Solicitante_Material = "Solicitante_Material";
        //public static readonly string Solicitante_Sustancias = "Solicitante_Sustancias";
        public const string Administrador = "Administrador";
        public const string Administrador_Almacen = "Administrador_Almacen";
        public const string Administrador_Informacion = "Administrador_Informacion";
        public const string Autorizador_Material = "Autorizador_Material";
        public const string Autorizador_Sustancias = "Autorizador_Sustancias";
        public const string Solicitante_Material = "Solicitante_Material";
        public const string Solicitante_Sustancias = "Solicitante_Sustancias";
    }

    public enum EnumAlmacenMovimiento
    {
        Entrada = 1,
        Salida = 2,
        Devolucion = 3

    }

    public enum EnumAlertaUnidad
    {
        Porcentaje = 1,
        Dias = 2

    }

}