using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Almacen.Infraestructura;
using AutoMapper;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Web;
using Almacen.Web.ViewModels;
using Almacen.Web.ViewModels.Catalogos;
using Almacen.Data;
using Almacen.Web.Aplicacion;

namespace Almacen.Web.Applicacion
{

    public class SolicitudMaterialesServicio
    {
        private readonly IGenericUnitOfWork _UoW;

        public SolicitudMaterialesServicio(DbContext entities)
        {
            _UoW = new GenericUnitOfWork(entities);

        }
        public SolicitudMaterialesServicio(IGenericUnitOfWork UoW)
        {
            _UoW = UoW;
        }

        public SolicitudVM Agregar(SolicitudVM entity)
        {
            SolicitudVM result = null;

            entity.FG = DateTime.Now;
            entity.UG = HttpContext.Current.User.Identity.GetUserId();
            entity.ST = true;

            entity.Folio = GenNumFolio();
            entity.idSolicitante = HttpContext.Current.User.Identity.GetUserId();

            if (entity.Materiales != null)
            {
                foreach (SolicitudDetalleVM item in entity.Materiales)
                {
                    item.FG = DateTime.Now;
                    item.UG = HttpContext.Current.User.Identity.GetUserId();
                    item.ST = true;
                }
            }


            tSolicitud convert = AutoMapperGenericsHelper<SolicitudVM, tSolicitud>.ConvertToDBEntity(entity);

            convert=_UoW.Repository<tSolicitud>().Agregar(convert);

            result = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(convert);

            return result;
        }

        public int Contar(Expression<Func<SolicitudVM, bool>> where)
        {
            int result = 0;

            //comentar en caso de obtener tambien los registros inactivos
            Expression<Func<SolicitudVM, bool>> Where2 = w => w.ST == true;
            where = PredicateBuilder.And(Where2, where);

            Expression<Func<tSolicitud, bool>> domainPredicate = AutoMapperGenericsHelper<Expression<Func<SolicitudVM, bool>>, Expression<Func<tSolicitud, bool>>>.ConvertToDBEntity(where);

            _UoW.Repository<tSolicitud>().Contar(domainPredicate);

            return result;
        }

        public void Editar(SolicitudVM model, int id, bool EstableceEstatus=false)
        {

            //tSolicitud entity_db = new RepositoryGenerico<tSolicitud>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);
            tSolicitud entity_db = _UoW.Repository<tSolicitud>().ObtenerPorId(id);

            //SolicitudVM tmp = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(entity_db);

            entity_db.Descripcion = model.Descripcion;
            entity_db.DescripcionUso = model.DescripcionUso;
            if (!string.IsNullOrEmpty(model.AccionCorrectiva))
            {
                entity_db.AccionCorrectiva = model.AccionCorrectiva;
            }

            var eliminar = model.Materiales.Select(s => s.idSustanciaMaterial);

            //eliminar
            var listaeliminar = entity_db.tSolicitudDetalle.Where(w => !eliminar.Contains(w.idSustanciaMaterial));

            foreach (tSolicitudDetalle item in listaeliminar)
            {

                item.FB = DateTime.Now;
                item.UB = HttpContext.Current.User.Identity.GetUserId();
                item.ST = false;
                _UoW.Repository<tSolicitudDetalle>().Eliminar(item);

            }

            //agregar o modificar

            foreach (SolicitudDetalleVM item in model.Materiales)
            {
                tSolicitudDetalle el = entity_db.tSolicitudDetalle.Where(w=> w.idSolicitudDetalle==item.idSolicitudDetalle).FirstOrDefault();

                if (el!=null)
                {
                    el.CantidadSolicitada = item.CantidadSolicitada;
                    el.CantidadAutorizada = item.CantidadAutorizada;
                    el.CantidadSurtida = item.CantidadSurtida;
                    el.CantidadDevolver = item.CantidadDevolver;
                    el.CantidadDevuelta = item.CantidadDevuelta;
                    el.idTipoEnvase = item.idTipoEnvase;
                    el.idPresentacion = item.idPresentacion;
                    el.idUnidadMedida = item.idUnidadMedida;

                    if (item.idAlmacen!=null)
                    {
                        el.idAlmacen = item.idAlmacen;
                    }

                    EditarLotes(item, el);

                    _UoW.Repository<tSolicitudDetalle>().Editar(el);
                }
                else
                {
                    tSolicitudDetalle tsd = AutoMapperGenericsHelper<SolicitudDetalleVM, tSolicitudDetalle>.ConvertToDBEntity(item);
                    tsd.idSolicitud = entity_db.idSolicitud;
                    tsd.FG = DateTime.Now;
                    tsd.UG = HttpContext.Current.User.Identity.GetUserId();
                    tsd.ST = true;

                    EditarLotes(item, tsd);

                    entity_db.tSolicitudDetalle.Add(tsd);
                    
                    _UoW.Repository<tSolicitudDetalle>().Agregar(tsd);
                }

                
            }

            entity_db.FM = DateTime.Now;
            entity_db.UM = HttpContext.Current.User.Identity.GetUserId();


            bool generarSalidaAlmacen = false;

            if (EstableceEstatus)
            {
                entity_db.cEstatusSolicitud = null;
                entity_db = LogicaAplicacion.EstableceEstatus(entity_db);

                //En caso de que ya este firmada se asigna la Firma del Solicitante
                //Segenera la salida del almacen
                if (!string.IsNullOrEmpty(model.FirmaSolicitante))
                {
                    entity_db.FirmaSolicitante = model.FirmaSolicitante;
                    generarSalidaAlmacen = true;
                }

            }

            //tSolicitud convert = AutoMapperGenericsHelper<SolicitudVM, tSolicitud>.ConvertToDBEntity(tmp);
            _UoW.Repository<tSolicitud>().Editar(entity_db);

            if (generarSalidaAlmacen)
            {
                AlmacenEntradasSalidas(model.idEstatusSolicitud, entity_db);
            }


        }

        private void AlmacenEntradasSalidas(int idEstatusSolicitud, tSolicitud entity_db)
        {
            // ***** Movimientos en el almacen *****
            // entity_db representa a la solicitud (tSolicitud)
            // entity_db ya debe contener los tSolicitudDetalle y tSolicitudDetalleLote previamente actualizados 
            // como si ya estuvieran guardados en bd

            switch (idEstatusSolicitud)
            {
                case (int)EnumEstatus.Pendiente:

                    foreach (var item in entity_db.tSolicitudDetalle)
                    {
                        foreach (var lote in item.tSolicitudDetalleLote)
                        {

                            var almacen = _UoW.Repository<tAlmacen>().ObtenerPorId(lote.idAlmacen);
                            if (almacen != null)
                            {
                                almacen.Existencia = (almacen.Existencia - lote.CantidadSurtida);
                                _UoW.Repository<tAlmacen>().Editar(almacen);

                                tAlmacenMovimiento mov = new tAlmacenMovimiento();

                                mov.idAlmacen = almacen.idAlmacen;
                                mov.Cantidad = lote.CantidadSurtida;
                                mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Salida;

                                mov.FG = DateTime.Now;
                                mov.UG = HttpContext.Current.User.Identity.GetUserId();
                                mov.ST = true;

                                _UoW.Repository<tAlmacenMovimiento>().Agregar(mov);
                            }
                        }
                    }

                    break;
                case (int)EnumEstatus.SurtidaDevolucion:

                    foreach (var item in entity_db.tSolicitudDetalle)
                    {
                        foreach (var lote in item.tSolicitudDetalleLote)
                        {
                            var almacen = _UoW.Repository<tAlmacen>().ObtenerPorId(lote.idAlmacen);
                            if (almacen != null)
                            {
                                almacen.Existencia = (almacen.Existencia + lote.CantidadDevuelta.Value);
                                _UoW.Repository<tAlmacen>().Editar(almacen);

                                tAlmacenMovimiento mov = new tAlmacenMovimiento();

                                mov.idAlmacen = almacen.idAlmacen;
                                mov.Cantidad = lote.CantidadDevuelta.Value;
                                mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Devolucion;

                                mov.FG = DateTime.Now;
                                mov.UG = HttpContext.Current.User.Identity.GetUserId();
                                mov.ST = true;

                                _UoW.Repository<tAlmacenMovimiento>().Agregar(mov);
                            }
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void EditarLotes(SolicitudDetalleVM item, tSolicitudDetalle el)
        {

            ////Eliminar lotes
            //var eliminar = item.tSolicitudDetalleLote.Select(s => s.idSolicitudDetalle);

            ////eliminar sustancias
            //var listaeliminar = el.tSolicitudDetalleLote.Where(w => !eliminar.Contains(w.idSolicitudDetalle));

            //foreach (tSolicitudDetalleLote lote in listaeliminar)
            //{
            //    item.FB = DateTime.Now;
            //    item.UB = HttpContext.Current.User.Identity.GetUserId();
            //    item.ST = false;
            //    _UoW.Repository<tSolicitudDetalle>().Eliminar(item);

            //}

            //Actualizar y agregar lotes
            foreach (var lote in item.tSolicitudDetalleLote)
            {
                tSolicitudDetalleLote lote_db = el.tSolicitudDetalleLote.Where(w => w.idSolicitudDetalleLote == lote.idSolicitudDetalleLote).FirstOrDefault();
                if (lote_db != null && lote.idSolicitudDetalleLote > 0)
                {
                    lote_db.CantidadSurtida = lote.CantidadSurtida;
                    lote_db.CantidadDevolver = lote.CantidadDevolver;
                    lote_db.CantidadDevuelta = lote.CantidadDevuelta;
                    lote_db.idAlmacen = lote.idAlmacen;

                    _UoW.Repository<tSolicitudDetalleLote>().Editar(lote_db);

                }
                else
                {

                    tSolicitudDetalleLote newlote = AutoMapperGenericsHelper<SolicitudDetalleLoteVM, tSolicitudDetalleLote>.ConvertToDBEntity(lote);

                    newlote.idSolicitudDetalle = el.idSolicitudDetalle;
                    newlote.idAlmacen = lote.idAlmacen;
                    newlote.FG = DateTime.Now;
                    newlote.UG = HttpContext.Current.User.Identity.GetUserId();
                    newlote.ST = true;

                    el.tSolicitudDetalleLote.Add(newlote);

                    _UoW.Repository<tSolicitudDetalleLote>().Agregar(newlote);
                }
            }
        }

        public void Eliminar(object id)
        {

            //tSolicitud entity_db = new RepositoryGenerico<tSolicitud>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);

            //SolicitudVM entity = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(entity_db);
            tSolicitud entity_db =_UoW.Repository<tSolicitud>().ObtenerPorId(id);

            entity_db.FB = DateTime.Now;
            entity_db.UB = HttpContext.Current.User.Identity.GetUserId();
            entity_db.ST = false;

            //tSolicitud convert = AutoMapperGenericsHelper<SolicitudVM, tSolicitud>.ConvertToDBEntity(entity);

            _UoW.Repository<tSolicitud>().Eliminar(entity_db);

        }

        public void EnviarAutorizar(int idSolicitud)
        {
            tSolicitud entity_db = new RepositoryGenerico<tSolicitud>(new CndAlmacenDopajeEntities()).ObtenerPorId(idSolicitud);

            SolicitudVM tmp = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(entity_db);

            tmp.cEstatusSolicitud = null;

            tmp = LogicaAplicacion.EstableceEstatus(tmp);

            tSolicitud convert = AutoMapperGenericsHelper<SolicitudVM, tSolicitud>.ConvertToDBEntity(tmp);

            _UoW.Repository<tSolicitud>().Editar(convert);
        }
   
        public void CambiarEstatus(int idSolicitud,EnumEstatus idEstatus)
        {
            tSolicitud entity_db = new RepositoryGenerico<tSolicitud>(new CndAlmacenDopajeEntities()).ObtenerPorId(idSolicitud);

            SolicitudVM tmp = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(entity_db);

            tmp.cEstatusSolicitud = null;

            tmp = LogicaAplicacion.EstableceEstatus(tmp, idEstatus);

            tmp.FM = DateTime.Now;
            tmp.UM = HttpContext.Current.User.Identity.GetUserId();

            tSolicitud convert = AutoMapperGenericsHelper<SolicitudVM, tSolicitud>.ConvertToDBEntity(tmp);

            _UoW.Repository<tSolicitud>().Editar(convert);
        }

        public List<SolicitudVM> EncontrarPor(ParametrosQuery<SolicitudVM> parametrosDeQuery)
        {
            List<SolicitudVM> result = null;

            Expression<Func<SolicitudVM, bool>> Where2 = w => w.ST == true;
            parametrosDeQuery.Where = PredicateBuilder.And(Where2, parametrosDeQuery.Where);

            ParametrosQuery<tSolicitud> domainPredicate = AutoMapperGenericsHelper<ParametrosQuery<SolicitudVM>, ParametrosQuery<tSolicitud>>.ConvertToDBEntity(parametrosDeQuery);

            IEnumerable<tSolicitud> entitys = _UoW.Repository<tSolicitud>().EncontrarPor(domainPredicate.Where);

            result = AutoMapperGenericsHelper<IEnumerable<tSolicitud>, List<SolicitudVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public SolicitudVM Obtener(Expression<Func<tSolicitud, bool>> predicate)
        {
            SolicitudVM result = null;

            tSolicitud entity = _UoW.Repository<tSolicitud>().Obtener(predicate);

            result = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(entity);

            result.Materiales = result.Materiales.Where(w => w.ST == true).ToList();
            result.Materiales.AsParallel().ForAll(a => a.idCategoria = result.idCategoria);

            return result;
        }

        public SolicitudVM ObtenerPorId(object id)
        {
            SolicitudVM result = null;

            tSolicitud entity = _UoW.Repository<tSolicitud>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<tSolicitud, SolicitudVM>.ConvertToDBEntity(entity);

            //using (LeerCatalogosServicio<SolicitudVM, tSolicitud> db = new LeerCatalogosServicio<SolicitudVM, tSolicitud>())
            //{
            //    result.tSustanciaMaterial = db.ObtenerPorId(result.idSustanciaMaterial);
            //}

            result.Materiales = result.Materiales.Where(w=> w.ST==true).ToList();
            result.Materiales.AsParallel().ForAll(a => a.idCategoria = result.idCategoria);

            return result;
        }

        public List<SolicitudVM> ObtenerTodos(Expression<Func<tSolicitud, bool>> predicate = null)
        {
            List<SolicitudVM> result = null;

            predicate=predicate.And(w => w.ST == true);

            IEnumerable<tSolicitud> entitys = _UoW.Repository<tSolicitud>().ObtenerTodos(predicate);

            result = AutoMapperGenericsHelper<IEnumerable<tSolicitud>, List<SolicitudVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public BaseListaCatalogosVM<SolicitudVM> ObtenerPaginado(ParametrosQuery<tSolicitud> parametrosDeQuery)
        {
            BaseListaCatalogosVM<SolicitudVM> result = new BaseListaCatalogosVM<SolicitudVM>();

            if (parametrosDeQuery.Where != null)
            {
                parametrosDeQuery.Where=parametrosDeQuery.Where.And(w => w.ST == true);
            }
            else
            {
                parametrosDeQuery.Where = w => w.ST == true;
            }

            Paginacion<tSolicitud> paginado = _UoW.Repository<tSolicitud>().ObtenerPaginado(parametrosDeQuery);


            result.Paginacion = AutoMapperGenericsHelper<Paginacion<tSolicitud>, Paginacion<SolicitudVM>>.ConvertToDBEntity(paginado);

            result.Paginador = Mapper.Map<Paginacion<SolicitudVM>, PaginadorViewModels>(result.Paginacion);

            return result;
        }

        public List<AlmacenMaterialVM> ObtenerExistencias(int idSustanciaMaterial, Expression<Func<tAlmacenMaterial, bool>> predicateExistencias = null)
        {
            List<AlmacenMaterialVM> result = new List<AlmacenMaterialVM>();

            predicateExistencias = predicateExistencias.And(w => w.ST == true && w.Existencia > 0);

            IEnumerable<tAlmacenMaterial> entitys = _UoW.Repository<tAlmacenMaterial>().ObtenerTodos(predicateExistencias);

            result = AutoMapperGenericsHelper<IEnumerable<tAlmacenMaterial>, List<AlmacenMaterialVM>>.ConvertToDBEntity(entitys);

            return result.OrderBy(o => o.FechaRecepcion).ToList();
        }

        public int Existencias(int idSustanciaMaterial, Expression<Func<tAlmacenMaterial, bool>> predicateExistencias = null)
        {
            int result = 0;

            predicateExistencias = predicateExistencias.And(w => w.ST == true);

            result = _UoW.Repository<tAlmacenMaterial>().EncontrarPor(predicateExistencias).Sum(su => su.Existencia);

            return result;
        }
        private static void FiltrosYSoloActivos(Expression<Func<tSolicitud, bool>> parametros)
        {

            Expression<Func<SolicitudVM, bool>> Where2 = w => w.ST == true;

            var lambda = Where2;

            if (parametros != null)
            {
                var domainPredicate = AutoMapperGenericsHelper<Expression<Func<tSolicitud, bool>>, Expression<Func<SolicitudVM, bool>>>.ConvertToDBEntity(parametros);
                lambda = PredicateBuilder.And(Where2, domainPredicate);
            }

            parametros = AutoMapperGenericsHelper<Expression<Func<SolicitudVM, bool>>, Expression<Func<tSolicitud, bool>>>.ConvertToDBEntity(lambda);
        }

        public ResultSave Save()
        {
            return _UoW.SaveChanges();
        }

        public int GenNumFolio()
        {
            int result = 1;

            tSolicitud material = _UoW.Repository<tSolicitud>().EncontrarPor().OrderByDescending(o => o.Folio).Take(1).FirstOrDefault();

            if (material != null)
            {
                result = material.Folio + 1;
            }

            return result;
        }


        private static Expression<Func<tSolicitud, TType>> MapDomainModelExpression<TType>(Expression<Func<SolicitudVM, TType>> expression)
        {
            return Mapper.Map<Expression<Func<SolicitudVM, TType>>, Expression<Func<tSolicitud, TType>>>(expression);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _UoW.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}