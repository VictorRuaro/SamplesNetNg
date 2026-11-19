using Almacen.Data;
using Almacen.Infraestructura;
using Almacen.Web.ViewModels;
using Almacen.Web.ViewModels.Catalogos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Almacen.Web
{
    public class Source<T>
    {
        public T Value { get; set; }
    }

    public class Destination<T>
    {
        public T Value { get; set; }
    }
    public static class AutoMapperConfig
    {
        

        public static void RegisterMappings()
        {
            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.ShouldMapField = ValoresQueryString => false;
                

                cfg.CreateMap(typeof(Source<>), typeof(Destination<>)).IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap(typeof(Destination<>), typeof(Source<>)).IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                //Paginacion
                cfg.CreateMap(typeof(Paginacion<>), typeof(PaginadorViewModels));
                //cfg.CreateMap<Paginacion<SustanciaVM>, PaginadorViewModels>().ForMember(m => m.ValoresQueryString, opt => opt.Ignore());
                //cfg.CreateMap<Paginacion<MaterialVM>, PaginadorViewModels>().ForMember(m => m.ValoresQueryString, opt => opt.Ignore());

                //Usuarios

                //cfg.CreateMap<Usuario, AspNetUsers>().IgnoreAllNonExisting();
                cfg.CreateMap<AspNetUsers, Usuario>().Ignore( i=> i.ST);

                //General
                cfg.CreateMap<CategoriaVM, cCategoria>().ReverseMap();

                //Catalogos para sustancia
                cfg.CreateMap<GrupoFarmacologicoVM, cGrupoFarmacologico>().ReverseMap();
                cfg.CreateMap<TipoSalVM, cTipoSal>().ReverseMap();
                cfg.CreateMap<ConservacionVM, cConservacion>().ReverseMap();
                cfg.CreateMap<DisolventeVM, cDisolvente>().ReverseMap();
                cfg.CreateMap<UbicacionVM, cUbicacion>().ReverseMap();
                cfg.CreateMap<TipoEnvaseVM, cTipoEnvase>().ReverseMap();
                cfg.CreateMap<UMCantidadEnvaseVM, cUMCantidadEnvase>().ReverseMap();
                cfg.CreateMap<UMConcentracionVM, cUMConcentracion>().ReverseMap();
                cfg.CreateMap<FabricanteVM, cProveedor>().ReverseMap();

                //Catalogos para materiales

                cfg.CreateMap<GradoVM, cGrado>().ReverseMap();
                cfg.CreateMap<cCondicionesAlmacenaje, CondicionesAlmacenajeVM>().ReverseMap();
                cfg.CreateMap<cMarca, MarcaVM>().ReverseMap();
                cfg.CreateMap<cPresentacion, PresentacionVM>().ReverseMap();
                cfg.CreateMap<ProveedorVM, cProveedor>().ReverseMap();

                //Catalogo de sustancias

                cfg.CreateMap<SustanciaVM, tDetalleSustacia>()
                .ForMember(dest => dest.cGrupoFarmacologico, opt => opt.MapFrom(src => src.cGrupoFarmacologico))
                .ForMember(dest => dest.cCategoria, opt => opt.MapFrom(src => src.cCategoria));
                

                cfg.CreateMap<tDetalleSustacia, SustanciaVM>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                //Catalogo de materiales

                cfg.CreateMap<MaterialVM, tDetalleMaterial>()
                .ForMember(dest => dest.cGrado, opt => opt.MapFrom(src => src.cGrado))
                .ForMember(dest => dest.cCategoria, opt => opt.MapFrom(src => src.cCategoria));

                cfg.CreateMap<tDetalleMaterial, MaterialVM>();

                //Almacen sustancias

                cfg.CreateMap<tAlmacenSustancia, AlmacenSustanciaVM>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverSustancia(src.tSustanciaMaterial)));

                cfg.CreateMap<AlmacenSustanciaVM, tAlmacenSustancia>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverSustancia_db(src.tSustanciaMaterial)));

                cfg.CreateMap<tAlmacenCertificado, AlmacenCertificadoVM>().IgnoreAllPropertiesWithAnInaccessibleSetter();

                cfg.CreateMap<AlmacenCertificadoVM, tAlmacenCertificado>().IgnoreAllPropertiesWithAnInaccessibleSetter();

                //Almacen materiales

                cfg.CreateMap<tAlmacenMaterial, AlmacenMaterialVM>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverMaterial(src.tSustanciaMaterial)));

                cfg.CreateMap<AlmacenMaterialVM, tAlmacenMaterial>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverMaterial_db(src.tSustanciaMaterial)));

                //Solicitud


                cfg.CreateMap<EstatusSolicitudVM, cEstatusSolicitud>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<cEstatusSolicitud, EstatusSolicitudVM>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                cfg.CreateMap<SolicitudDetalleLoteVM, tSolicitudDetalleLote>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<tSolicitudDetalleLote, SolicitudDetalleLoteVM>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                cfg.CreateMap<SolicitudDetalleVM, tSolicitudDetalle>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<tSolicitudDetalle, SolicitudDetalleVM>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                cfg.CreateMap<SolicitudVM, tSolicitud>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<tSolicitud, SolicitudVM>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                cfg.CreateMap<tSolicitud, SolicitudVM>()
                .ForMember(dest => dest.Materiales, opt => opt.ResolveUsing(src => src.tSolicitudDetalle));
                cfg.CreateMap<SolicitudVM, tSolicitud>()
                .ForMember(dest => dest.tSolicitudDetalle, opt => opt.ResolveUsing(src => src.Materiales));


                //Devolucion sustancias

                cfg.CreateMap<tDevolucionSustancia, DevolucionSustanciaVM>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverSustancia(src.tSustanciaMaterial)));

                cfg.CreateMap<DevolucionSustanciaVM, tDevolucionSustancia>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverSustancia_db(src.tSustanciaMaterial)));


                cfg.CreateMap<AlmacenSustanciaVM, DevolucionSustanciaVM>()
                .ReverseMap()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                

                //Devolucion materiales

                cfg.CreateMap<tDevolucionMaterial, DevolucionMaterialVM>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverMaterial(src.tSustanciaMaterial)));

                cfg.CreateMap<DevolucionMaterialVM, tDevolucionMaterial>()
                .ForMember(dest => dest.tSustanciaMaterial, opt => opt.ResolveUsing(src => ResolverMaterial_db(src.tSustanciaMaterial)));

                cfg.CreateMap<tDevolucionSustancia, tAlmacenSustancia>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<tDevolucionMaterial, tAlmacenMaterial>().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                cfg.CreateMap<AlmacenMaterialVM, DevolucionMaterialVM>()
                .ReverseMap()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

                // Alertas
                cfg.CreateMap<AlertaUnidadVM, tAlertaUnidad>().ReverseMap();
                cfg.CreateMap<AlertaXSustanciaMaterialVM, tAlertaXSustanciaMaterial>().ReverseMap();

                cfg.CreateMap<Alerta, tAlerta>().ReverseMap();
                cfg.CreateMap<AlertaDestinatarioVM, tAlertaDestinatario>().ReverseMap();


            });

        }

        public static MaterialVM ResolverMaterial(tSustanciaMaterial source)
        {
            MaterialVM result = Mapper.Map<tSustanciaMaterial, MaterialVM>(source);

            return result;
        }

        public static tSustanciaMaterial ResolverMaterial_db(MaterialVM source)
        {
            tSustanciaMaterial result = Mapper.Map<MaterialVM, tSustanciaMaterial>(source);

            return result;
        }

        public static SustanciaVM ResolverSustancia(tSustanciaMaterial source)
        {
            SustanciaVM result = Mapper.Map<tSustanciaMaterial, SustanciaVM>(source);

            return result;
        }
        public static tSustanciaMaterial ResolverSustancia_db(SustanciaVM source)
        {
            tSustanciaMaterial result = Mapper.Map<SustanciaVM, tSustanciaMaterial>(source);

            return result;
        }

        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> map,
        Expression<Func<TDestination, object>> selector)
        {
            map.ForMember(selector, config => config.Ignore());
            return map;
        }

        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            var cfg = Mapper.Configuration;

            var existingMaps = cfg.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType)
                && x.DestinationType.Equals(destinationType));

            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }


    }
}