using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Almacen.Data;
using Almacen.Web.ViewModels;
using System.Web.Routing;
using Almacen.Web.Tools;
using Almacen.Web.ViewModels.Reportes;
using Almacen.Web.ViewModels.Catalogos;
using Almacen.Web.Applicacion;
using Almacen.Web.Aplicacion;

namespace Almacen.Web.Controllers
{
    public class ReportesController : Controller
    {
        protected int idCategoriaSustancias
        {
            get
            {
                return (int)EnumCategoria.Sustancias;
            }
        }

        protected int idCategoriaMateriales
        {
            get
            {
                return (int)EnumCategoria.Materiales;
            }
        }
        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reportes/Sustancias
        public ActionResult Sustancias()
        {
            return View();
        }

        public ActionResult RSInventario(Rpt_Sustancia_FiltrosVM Filtros,int pagina=1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_Inventario_Sustancias_Result> model = new BaseListaReportesVM<sp_Rpt_Inventario_Sustancias_Result>();

            List<sp_Rpt_Inventario_Sustancias_Result> listaInventario = new List<sp_Rpt_Inventario_Sustancias_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;

            
            

            //Listas para DropDownList
            List<GrupoFarmacologicoVM> ListaGrupoFarmacologico = new List<GrupoFarmacologicoVM>();

            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico>(db).ObtenerTodos();

                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                //ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                //ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo
                listaInventario = db.sp_Rpt_Inventario_Sustancias(
                    Filtros.Codigo, 
                    Filtros.Descripcion, 
                    Filtros.Sinonimos,
                    Filtros.idGrupoFarmacologico,
                    Filtros.CAS,
                    Filtros.PMCompuesto,
                    Filtros.FormulaMolecular,
                    Filtros.LoteInterno,
                    Filtros.idTipoSal,
                    Filtros.idProveedor,
                    Filtros.Referencia,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.FechaCaducidad,
                    Filtros.idConservacion,
                    Filtros.idUbicacion,
                    Filtros.idTipoEnvase,
                    Filtros.idDisolvente,
                    Filtros.Observaciones,
                    Filtros.EsSustanciaControlada,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if (listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }
            else
            {
                model.Paginador.TotalRegistros = 0;
                model.SinDatosListado = true;
            }

            //Datos para Filtros

            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrupoFarmacologico", "GrupoFarmacologico", true, Filtros.idGrupoFarmacologico);

            ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");
            ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
            ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");
            ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");

            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["DescripcionUso"] = Filtros.Sinonimos;
            model.Paginador.ValoresQueryString["idGrupoFarmacologico"] = Filtros.idGrupoFarmacologico;
            model.Paginador.ValoresQueryString["CAS"] = Filtros.CAS;
            model.Paginador.ValoresQueryString["PMCompuesto"] = Filtros.PMCompuesto;

            model.Paginador.ValoresQueryString["FormulaMolecular"] = Filtros.FormulaMolecular;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["idTipoSal"] = Filtros.idTipoSal;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["Referencia"] = Filtros.Referencia;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idConservacion"] = Filtros.idConservacion;
            model.Paginador.ValoresQueryString["idUbicacion"] = Filtros.idUbicacion;
            model.Paginador.ValoresQueryString["idTipoEnvase"] = Filtros.idTipoEnvase;
            model.Paginador.ValoresQueryString["idDisolvente"] = Filtros.idDisolvente;
            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;
            model.Paginador.ValoresQueryString["EsSustanciaControlada"] = Filtros.EsSustanciaControlada;

            ViewBag.Filtros = Filtros;


            return View(model);

        }

        public ActionResult RSCaducos(Rpt_Sustancia_FiltrosVM Filtros, int pagina = 1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_Caducos_Sustancias_Result> model = new BaseListaReportesVM<sp_Rpt_Caducos_Sustancias_Result>();

            List<sp_Rpt_Caducos_Sustancias_Result> listaInventario = new List<sp_Rpt_Caducos_Sustancias_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;


            

            //Listas para DropDownList
            List<GrupoFarmacologicoVM> ListaGrupoFarmacologico = new List<GrupoFarmacologicoVM>();

            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico>(db).ObtenerTodos();

                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                //ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                //ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo 
                listaInventario = db.sp_Rpt_Caducos_Sustancias(
                    Filtros.Codigo,
                    Filtros.Descripcion,
                    Filtros.Sinonimos,
                    Filtros.idGrupoFarmacologico,
                    Filtros.CAS,
                    Filtros.PMCompuesto,
                    Filtros.FormulaMolecular,
                    Filtros.LoteInterno,
                    Filtros.idTipoSal,
                    Filtros.idProveedor,
                    Filtros.Referencia,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.idConservacion,
                    Filtros.idUbicacion,
                    Filtros.idTipoEnvase,
                    Filtros.idDisolvente,
                    Filtros.Observaciones,
                    Filtros.EsSustanciaControlada,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if (listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }
            else
            {
                model.Paginador.TotalRegistros = 0;
                model.SinDatosListado = true;
            }

            //Datos para Filtros

            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrupoFarmacologico", "GrupoFarmacologico", true, Filtros.idGrupoFarmacologico);

            ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");
            ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
            ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");
            ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");

            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["DescripcionUso"] = Filtros.Sinonimos;
            model.Paginador.ValoresQueryString["idGrupoFarmacologico"] = Filtros.idGrupoFarmacologico;
            model.Paginador.ValoresQueryString["CAS"] = Filtros.CAS;
            model.Paginador.ValoresQueryString["PMCompuesto"] = Filtros.PMCompuesto;

            model.Paginador.ValoresQueryString["FormulaMolecular"] = Filtros.FormulaMolecular;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["idTipoSal"] = Filtros.idTipoSal;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["Referencia"] = Filtros.Referencia;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idConservacion"] = Filtros.idConservacion;
            model.Paginador.ValoresQueryString["idUbicacion"] = Filtros.idUbicacion;
            model.Paginador.ValoresQueryString["idTipoEnvase"] = Filtros.idTipoEnvase;
            model.Paginador.ValoresQueryString["idDisolvente"] = Filtros.idDisolvente;
            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;
            model.Paginador.ValoresQueryString["EsSustanciaControlada"] = Filtros.EsSustanciaControlada;

            ViewBag.Filtros = Filtros;

            ViewBag.RptCaducos = true; //se utiliza para ocultar en el partial de los filtros, el filtro de fecha caducidad

            return View(model);

        }

        public ActionResult RSSinExistencia(Rpt_Sustancia_FiltrosVM Filtros, int pagina = 1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_SinExistencia_Sustancias_Result> model = new BaseListaReportesVM<sp_Rpt_SinExistencia_Sustancias_Result>();

            List<sp_Rpt_SinExistencia_Sustancias_Result> listaInventario = new List<sp_Rpt_SinExistencia_Sustancias_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;


            

            //Listas para DropDownList
            List<GrupoFarmacologicoVM> ListaGrupoFarmacologico = new List<GrupoFarmacologicoVM>();

            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico>(db).ObtenerTodos();

                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                //ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                //ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo
                listaInventario = db.sp_Rpt_SinExistencia_Sustancias(
                    Filtros.Codigo,
                    Filtros.Descripcion,
                    Filtros.Sinonimos,
                    Filtros.idGrupoFarmacologico,
                    Filtros.CAS,
                    Filtros.PMCompuesto,
                    Filtros.FormulaMolecular,
                    Filtros.LoteInterno,
                    Filtros.idTipoSal,
                    Filtros.idProveedor,
                    Filtros.Referencia,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.FechaCaducidad,
                    Filtros.idConservacion,
                    Filtros.idUbicacion,
                    Filtros.idTipoEnvase,
                    Filtros.idDisolvente,
                    Filtros.Observaciones,
                    Filtros.EsSustanciaControlada,
                    Filtros.IncluirPorcentajeMinimo,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if (listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }
            else
            {
                model.Paginador.TotalRegistros = 0;
                model.SinDatosListado = true;
            }

            //Datos para Filtros

            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrupoFarmacologico", "GrupoFarmacologico", true, Filtros.idGrupoFarmacologico);

            ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");
            ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
            ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");
            ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");

            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["DescripcionUso"] = Filtros.Sinonimos;
            model.Paginador.ValoresQueryString["idGrupoFarmacologico"] = Filtros.idGrupoFarmacologico;
            model.Paginador.ValoresQueryString["CAS"] = Filtros.CAS;
            model.Paginador.ValoresQueryString["PMCompuesto"] = Filtros.PMCompuesto;

            model.Paginador.ValoresQueryString["FormulaMolecular"] = Filtros.FormulaMolecular;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["idTipoSal"] = Filtros.idTipoSal;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["Referencia"] = Filtros.Referencia;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idConservacion"] = Filtros.idConservacion;
            model.Paginador.ValoresQueryString["idUbicacion"] = Filtros.idUbicacion;
            model.Paginador.ValoresQueryString["idTipoEnvase"] = Filtros.idTipoEnvase;
            model.Paginador.ValoresQueryString["idDisolvente"] = Filtros.idDisolvente;
            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;
            model.Paginador.ValoresQueryString["EsSustanciaControlada"] = Filtros.EsSustanciaControlada;
            model.Paginador.ValoresQueryString["IncluirPorcentajeMinimo"] = Filtros.IncluirPorcentajeMinimo;

            ViewBag.Filtros = Filtros;
            ViewBag.MenuSinExistencia = true;//para que se muestre en el partial de filtros el filtro de incluir porcentaje minimo

            return View(model);

        }

        public ActionResult RSDevoluciones(Rpt_Sustancia_FiltrosVM Filtros, int pagina = 1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_Devoluciones_Sustancias_Result> model = new BaseListaReportesVM<sp_Rpt_Devoluciones_Sustancias_Result>();

            List<sp_Rpt_Devoluciones_Sustancias_Result> listaInventario = new List<sp_Rpt_Devoluciones_Sustancias_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;


            

            //Listas para DropDownList
            List<GrupoFarmacologicoVM> ListaGrupoFarmacologico = new List<GrupoFarmacologicoVM>();

            List<TipoSalVM> ListaTipoSal = new List<TipoSalVM>();
            List<UbicacionVM> ListaUbicacion = new List<UbicacionVM>();
            List<TipoEnvaseVM> ListaTipoEnvase = new List<TipoEnvaseVM>();

            List<UMCantidadEnvaseVM> ListaUMCantidadEnvase = new List<UMCantidadEnvaseVM>();
            List<UMConcentracionVM> ListaUMConcentracion = new List<UMConcentracionVM>();
            List<DisolventeVM> ListaDisolvente = new List<DisolventeVM>();
            List<ConservacionVM> ListaConservacion = new List<ConservacionVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrupoFarmacologico = new LeerCatalogosServicio<GrupoFarmacologicoVM, cGrupoFarmacologico>(db).ObtenerTodos();

                ListaTipoSal = new LeerCatalogosServicio<TipoSalVM, cTipoSal>(db).ObtenerTodos();
                ListaUbicacion = new LeerCatalogosServicio<UbicacionVM, cUbicacion>(db).ObtenerTodos();
                ListaTipoEnvase = new LeerCatalogosServicio<TipoEnvaseVM, cTipoEnvase>(db).ObtenerTodos();
                //ListaUMCantidadEnvase = new LeerCatalogosServicio<UMCantidadEnvaseVM, cUMCantidadEnvase>(db).ObtenerTodos();

                //ListaUMConcentracion = new LeerCatalogosServicio<UMConcentracionVM, cUMConcentracion>(db).ObtenerTodos();
                ListaDisolvente = new LeerCatalogosServicio<DisolventeVM, cDisolvente>(db).ObtenerTodos();
                ListaConservacion = new LeerCatalogosServicio<ConservacionVM, cConservacion>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo
                listaInventario = db.sp_Rpt_Devoluciones_Sustancias(
                    Filtros.Codigo,
                    Filtros.Descripcion,
                    Filtros.Sinonimos,
                    Filtros.idGrupoFarmacologico,
                    Filtros.CAS,
                    Filtros.PMCompuesto,
                    Filtros.FormulaMolecular,
                    Filtros.LoteInterno,
                    Filtros.idTipoSal,
                    Filtros.idProveedor,
                    Filtros.Referencia,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.FechaCaducidad,
                    Filtros.idConservacion,
                    Filtros.idUbicacion,
                    Filtros.idTipoEnvase,
                    Filtros.idDisolvente,
                    Filtros.Observaciones,
                    Filtros.EsSustanciaControlada,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if(listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }else
            {
                model.Paginador.TotalRegistros =  0;
                model.SinDatosListado = true;
            }
            

            //Datos para Filtros

            ViewBag.idGrupoFarmacologico = CustomExtension.convertSelectList(ListaGrupoFarmacologico, "idGrupoFarmacologico", "GrupoFarmacologico", true, Filtros.idGrupoFarmacologico);

            ViewBag.idTipoSal = CustomExtension.convertSelectList(ListaTipoSal, "idTipoSal", "TipoSal");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idConservacion = CustomExtension.convertSelectList(ListaConservacion, "idConservacion", "Conservacion");
            ViewBag.idUbicacion = CustomExtension.convertSelectList(ListaUbicacion, "idUbicacion", "Ubicacion");
            ViewBag.idTipoEnvase = CustomExtension.convertSelectList(ListaTipoEnvase, "idTipoEnvase", "TipoEnvase");
            ViewBag.idDisolvente = CustomExtension.convertSelectList(ListaDisolvente, "idDisolvente", "Disolvente");

            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["DescripcionUso"] = Filtros.Sinonimos;
            model.Paginador.ValoresQueryString["idGrupoFarmacologico"] = Filtros.idGrupoFarmacologico;
            model.Paginador.ValoresQueryString["CAS"] = Filtros.CAS;
            model.Paginador.ValoresQueryString["PMCompuesto"] = Filtros.PMCompuesto;

            model.Paginador.ValoresQueryString["FormulaMolecular"] = Filtros.FormulaMolecular;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["idTipoSal"] = Filtros.idTipoSal;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["Referencia"] = Filtros.Referencia;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idConservacion"] = Filtros.idConservacion;
            model.Paginador.ValoresQueryString["idUbicacion"] = Filtros.idUbicacion;
            model.Paginador.ValoresQueryString["idTipoEnvase"] = Filtros.idTipoEnvase;
            model.Paginador.ValoresQueryString["idDisolvente"] = Filtros.idDisolvente;
            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;
            model.Paginador.ValoresQueryString["EsSustanciaControlada"] = Filtros.EsSustanciaControlada;

            ViewBag.Filtros = Filtros;


            return View(model);

        }

        // GET: Reportes/Materiales
        public ActionResult Materiales()
        {
            return View();
        }

        public ActionResult RMInventario(Rpt_Material_FiltrosVM Filtros, int pagina = 1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_Inventario_Materiales_Result> model = new BaseListaReportesVM<sp_Rpt_Inventario_Materiales_Result>();

            List<sp_Rpt_Inventario_Materiales_Result> listaInventario = new List<sp_Rpt_Inventario_Materiales_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;


            

            //Listas para DropDownList
            List<GradoVM> ListaGrado = new List<GradoVM>();

            List<MarcaVM> ListaMarca = new List<MarcaVM>();

            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();

            List<CondicionesAlmacenajeVM> ListaCondicionesAlmacenaje = new List<CondicionesAlmacenajeVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrado = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();

                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM, cPresentacion>(db).ObtenerTodos();
                ListaCondicionesAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM, cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo
                listaInventario = db.sp_Rpt_Inventario_Materiales(
                    Filtros.Codigo,
                    Filtros.Descripcion,
                    Filtros.idGrado,
                    Filtros.NumeroParte,
                    Filtros.LoteInterno,
                    Filtros.NumCatalago,
                    Filtros.idMarca,
                    Filtros.idPresentacion,
                    Filtros.idProveedor,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.FechaCaducidad,
                    Filtros.idCondicionesAlmacenaje,
                    Filtros.Observaciones,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if (listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }
            else
            {
                model.Paginador.TotalRegistros = 0;
                model.SinDatosListado = true;
            }

            //Datos para Filtros

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrado, "idGrado", "Grado", true, Filtros.idGrado);

            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCondicionesAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");
            
            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");
            

            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["idGrado"] = Filtros.idGrado;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["NumCatalago"] = Filtros.NumCatalago;
            model.Paginador.ValoresQueryString["idMarca"] = Filtros.idMarca;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idCondicionesAlmacenaje"] = Filtros.idCondicionesAlmacenaje;

            model.Paginador.ValoresQueryString["idPresentacion"] = Filtros.idPresentacion;

            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;


            ViewBag.Filtros = Filtros;


            return View(model);

        }

        public ActionResult RMCaducos(Rpt_Material_FiltrosVM Filtros, int pagina = 1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_Caducos_Materiales_Result> model = new BaseListaReportesVM<sp_Rpt_Caducos_Materiales_Result>();

            List<sp_Rpt_Caducos_Materiales_Result> listaInventario = new List<sp_Rpt_Caducos_Materiales_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;




            //Listas para DropDownList
            List<GradoVM> ListaGrado = new List<GradoVM>();

            List<MarcaVM> ListaMarca = new List<MarcaVM>();

            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();

            List<CondicionesAlmacenajeVM> ListaCondicionesAlmacenaje = new List<CondicionesAlmacenajeVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrado = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();

                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM, cPresentacion>(db).ObtenerTodos();
                ListaCondicionesAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM, cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo
                listaInventario = db.sp_Rpt_Caducos_Materiales(
                    Filtros.Codigo,
                    Filtros.Descripcion,
                    Filtros.idGrado,
                    Filtros.NumeroParte,
                    Filtros.LoteInterno,
                    Filtros.NumCatalago,
                    Filtros.idMarca,
                    Filtros.idPresentacion,
                    Filtros.idProveedor,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.idCondicionesAlmacenaje,
                    Filtros.Observaciones,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if (listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }
            else
            {
                model.Paginador.TotalRegistros = 0;
                model.SinDatosListado = true;
            }

            //Datos para Filtros

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrado, "idGrado", "Grado", true, Filtros.idGrado);

            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCondicionesAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");

            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");


            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["idGrado"] = Filtros.idGrado;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["NumCatalago"] = Filtros.NumCatalago;
            model.Paginador.ValoresQueryString["idMarca"] = Filtros.idMarca;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idCondicionesAlmacenaje"] = Filtros.idCondicionesAlmacenaje;

            model.Paginador.ValoresQueryString["idPresentacion"] = Filtros.idPresentacion;

            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;


            ViewBag.Filtros = Filtros;

            ViewBag.RptCaducos = true; //se utiliza para ocultar en el partial de los filtros, el filtro de fecha caducidad


            return View(model);

        }

        public ActionResult RMSinExistencia(Rpt_Material_FiltrosVM Filtros, int pagina = 1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_SinExistencia_Materiales_Result> model = new BaseListaReportesVM<sp_Rpt_SinExistencia_Materiales_Result>();

            List<sp_Rpt_SinExistencia_Materiales_Result> listaInventario = new List<sp_Rpt_SinExistencia_Materiales_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;




            //Listas para DropDownList
            List<GradoVM> ListaGrado = new List<GradoVM>();

            List<MarcaVM> ListaMarca = new List<MarcaVM>();

            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();

            List<CondicionesAlmacenajeVM> ListaCondicionesAlmacenaje = new List<CondicionesAlmacenajeVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrado = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();

                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM, cPresentacion>(db).ObtenerTodos();
                ListaCondicionesAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM, cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo
                listaInventario = db.sp_Rpt_SinExistencia_Materiales(
                    Filtros.Codigo,
                    Filtros.Descripcion,
                    Filtros.idGrado,
                    Filtros.NumeroParte,
                    Filtros.LoteInterno,
                    Filtros.NumCatalago,
                    Filtros.idMarca,
                    Filtros.idPresentacion,
                    Filtros.idProveedor,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.FechaCaducidad,
                    Filtros.idCondicionesAlmacenaje,
                    Filtros.Observaciones,
                    Filtros.IncluirPorcentajeMinimo,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if (listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }
            else
            {
                model.Paginador.TotalRegistros = 0;
                model.SinDatosListado = true;
            }

            //Datos para Filtros

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrado, "idGrado", "Grado", true, Filtros.idGrado);

            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCondicionesAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");

            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");


            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["idGrado"] = Filtros.idGrado;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["NumCatalago"] = Filtros.NumCatalago;
            model.Paginador.ValoresQueryString["idMarca"] = Filtros.idMarca;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idCondicionesAlmacenaje"] = Filtros.idCondicionesAlmacenaje;

            model.Paginador.ValoresQueryString["idPresentacion"] = Filtros.idPresentacion;

            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;
            model.Paginador.ValoresQueryString["IncluirPorcentajeMinimo"] = Filtros.IncluirPorcentajeMinimo;


            ViewBag.Filtros = Filtros;
            ViewBag.MenuSinExistencia = true;//para que se muestre en el partial de filtros el filtro de incluir porcentaje minimo

            return View(model);

        }

        public ActionResult RMDevoluciones(Rpt_Material_FiltrosVM Filtros, int pagina = 1)
        {
            int RegistrosPorPagina = 15;

            BaseListaReportesVM<sp_Rpt_Devoluciones_Materiales_Result> model = new BaseListaReportesVM<sp_Rpt_Devoluciones_Materiales_Result>();

            List<sp_Rpt_Devoluciones_Materiales_Result> listaInventario = new List<sp_Rpt_Devoluciones_Materiales_Result>();
            model.Paginador.PaginaActual = pagina;
            model.Paginador.RegistrosPorPagina = RegistrosPorPagina;




            //Listas para DropDownList
            List<GradoVM> ListaGrado = new List<GradoVM>();

            List<MarcaVM> ListaMarca = new List<MarcaVM>();

            List<PresentacionVM> ListaPresentacion = new List<PresentacionVM>();

            List<CondicionesAlmacenajeVM> ListaCondicionesAlmacenaje = new List<CondicionesAlmacenajeVM>();

            List<ProveedorVM> ListaProveedor = new List<ProveedorVM>();


            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                ListaGrado = new LeerCatalogosServicio<GradoVM, cGrado>(db).ObtenerTodos();

                ListaMarca = new LeerCatalogosServicio<MarcaVM, cMarca>(db).ObtenerTodos();
                ListaPresentacion = new LeerCatalogosServicio<PresentacionVM, cPresentacion>(db).ObtenerTodos();
                ListaCondicionesAlmacenaje = new LeerCatalogosServicio<CondicionesAlmacenajeVM, cCondicionesAlmacenaje>(db).ObtenerTodos();
                ListaProveedor = new LeerCatalogosServicio<ProveedorVM, cProveedor>(db).ObtenerTodos(w => w.idCategoria == this.idCategoriaSustancias);


                //llenado del modelo
                listaInventario = db.sp_Rpt_Devoluciones_Materiales(
                    Filtros.Codigo,
                    Filtros.Descripcion,
                    Filtros.idGrado,
                    Filtros.NumeroParte,
                    Filtros.LoteInterno,
                    Filtros.NumCatalago,
                    Filtros.idMarca,
                    Filtros.idPresentacion,
                    Filtros.idProveedor,
                    Filtros.LoteProveedor,
                    Filtros.FechaRecepcion,
                    Filtros.FechaCaducidad,
                    Filtros.idCondicionesAlmacenaje,
                    Filtros.Observaciones,
                    pagina,
                    RegistrosPorPagina
                    ).ToList();

            }

            model.Paginacion.Lista = listaInventario;

            //Paginador
            model.Paginador.PaginaActual = pagina;

            if (listaInventario.Any())
            {
                model.Paginador.TotalRegistros = listaInventario.FirstOrDefault().TotalRegistros.Value;
            }
            else
            {
                model.Paginador.TotalRegistros = 0;
                model.SinDatosListado = true;
            }

            //Datos para Filtros

            ViewBag.idGrado = CustomExtension.convertSelectList(ListaGrado, "idGrado", "Grado", true, Filtros.idGrado);

            ViewBag.idMarca = CustomExtension.convertSelectList(ListaMarca, "idMarca", "Marca");
            ViewBag.idProveedor = CustomExtension.convertSelectList(ListaProveedor, "idProveedor", "Proveedor");

            ViewBag.idCondicionesAlmacenaje = CustomExtension.convertSelectList(ListaCondicionesAlmacenaje, "idCondicionesAlmacenaje", "CondicionesAlmacenaje");

            ViewBag.idPresentacion = CustomExtension.convertSelectList(ListaPresentacion, "idPresentacion", "Presentacion");


            //Paginador

            model.Paginador.ValoresQueryString = new RouteValueDictionary();
            model.Paginador.ValoresQueryString["Folio"] = Filtros.Codigo;
            model.Paginador.ValoresQueryString["Descripcion"] = Filtros.Descripcion;
            model.Paginador.ValoresQueryString["idGrado"] = Filtros.idGrado;
            model.Paginador.ValoresQueryString["LoteInterno"] = Filtros.LoteInterno;
            model.Paginador.ValoresQueryString["NumCatalago"] = Filtros.NumCatalago;
            model.Paginador.ValoresQueryString["idMarca"] = Filtros.idMarca;
            model.Paginador.ValoresQueryString["idProveedor"] = Filtros.idProveedor;
            model.Paginador.ValoresQueryString["LoteProveedor"] = Filtros.LoteProveedor;

            model.Paginador.ValoresQueryString["FechaRecepcion"] = Filtros.FechaRecepcion;
            model.Paginador.ValoresQueryString["FechaCaducidad"] = Filtros.FechaCaducidad;
            model.Paginador.ValoresQueryString["idCondicionesAlmacenaje"] = Filtros.idCondicionesAlmacenaje;

            model.Paginador.ValoresQueryString["idPresentacion"] = Filtros.idPresentacion;

            model.Paginador.ValoresQueryString["Observaciones"] = Filtros.Observaciones;


            ViewBag.Filtros = Filtros;


            return View(model);

        }

    }
}
