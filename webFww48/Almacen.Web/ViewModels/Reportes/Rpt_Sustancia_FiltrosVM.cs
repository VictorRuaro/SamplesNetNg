using Almacen.Infraestructura;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Reportes
{
    

    public class Rpt_Sustancia_FiltrosVM
    {
        public int idSustanciaMaterial { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Sustancia")]
        public string Descripcion { get; set; }

        [Display(Name = "Sinonimos")]
        public string Sinonimos { get; set; }

        [Display(Name = "CAS")]
        public string CAS { get; set; }

        [Display(Name = "PM Compuesto")]
        public string PMCompuesto { get; set; }

        [Display(Name = "Formula Molecular")]
        public string FormulaMolecular { get; set; }

        [Display(Name = "Sustancia Controlada")]
        [UIHint("UISiNo")]
        public Nullable<bool> EsSustanciaControlada { get; set; }

        
        

        [Display(Name = "Grupo Farmacológico")]
        public Nullable<int> idGrupoFarmacologico { get; set; }

        //Almacen
        [Display(Name = "Fabricante")]
        public Nullable<int> idProveedor { get; set; }

        [Display(Name = "No. de Lote Interno")]
        public string LoteInterno { get; set; }

        [Display(Name = "Fecha Recepción")]
        public Nullable<DateTime> FechaRecepcion { get; set; }

        [Display(Name = "Fecha Caducidad")]
        public Nullable<DateTime> FechaCaducidad { get; set; }

        [Display(Name = "Lote del Proveedor")]
        public string LoteProveedor { get; set; }

        [Display(Name = "Observaciones")]
        [MaxLength(150, ErrorMessage = "El numero máximo de caracteres es de 150")]
        public string Observaciones { get; set; }

        //Propiedades Sustancia

        [Display(Name = "Tipo de Sal")]
        public Nullable<int> idTipoSal { get; set; }

        [Display(Name = "Referencia")]
        [MaxLength(50, ErrorMessage = "El numero máximo de caracteres es de 50")]
        public string Referencia { get; set; }

        [Display(Name = "Conservación")]
        public Nullable<int> idConservacion { get; set; }

        [Display(Name = "Ubicación")]
        public Nullable<int> idUbicacion { get; set; }

        [Display(Name = "Tipo de Envase")]
        public Nullable<int> idTipoEnvase { get; set; }

        //[Display(Name = "Cantidad por Envase")]
        //[Range(0, 9999999, ErrorMessage = "La {0} debe ser minimo de {1} y maximo {2}")]
        //public Nullable<int> CantidadPorEnvase { get; set; }

        //[Display(Name = "UM. Cantidad por Envase")]
        //public Nullable<int> idUMCantidadEnvase { get; set; }



        [Display(Name = "Disolvente")]
        public Nullable<int> idDisolvente { get; set; }


        [Display(Name = "Incluir con Porcentaje Minimo")]
        public Nullable<bool> IncluirPorcentajeMinimo { get; set; }


    }
}