using Almacen.Infraestructura;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Web.ViewModels.Reportes
{
    
    public class Rpt_Material_FiltrosVM
    {
        public Nullable<int> idSustanciaMaterial { get; set; }

        [Display(Name = "ID")]
        public string Codigo { get; set; }

        [Display(Name = "Material")]
        public string Descripcion { get; set; }

        [Display(Name = "Categoria")]
        public Nullable<int> idCategoria { get; set; }

        [Display(Name = "Grado")]
        public Nullable<int> idGrado { get; set; }

        [Display(Name = "Numero de Parte")]
        public string NumeroParte { get; set; }

        //Almacen
        [Display(Name = "Proveedor")]
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

        //Propiedades Material

        [Display(Name = "No. de Parte")]
        [MaxLength(20, ErrorMessage = "El numero máximo de caracteres es de 20")]
        public string NumParte { get; set; }

        [Display(Name = "No. de Catalogo")]
        [MaxLength(20, ErrorMessage = "El numero máximo de caracteres es de 20")]
        public string NumCatalago { get; set; }


        [Display(Name = "Marca")]
        public Nullable<int> idMarca { get; set; }

        [Display(Name = "Condiciones de Almacenaje")]
        public Nullable<int> idCondicionesAlmacenaje { get; set; }

        [Display(Name = "Presentación")]
        public Nullable<int> idPresentacion { get; set; }

        [Display(Name = "Incluir con Porcentaje Minimo")]
        public Nullable<bool> IncluirPorcentajeMinimo { get; set; }

    }
}