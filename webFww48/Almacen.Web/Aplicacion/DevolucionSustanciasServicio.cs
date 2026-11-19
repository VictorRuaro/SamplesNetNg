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
using Almacen.Web.Tools;

namespace Almacen.Web.Applicacion
{
    public class DevolucionSustanciasServicio : IDisposable
    {
        private readonly IGenericUnitOfWork _UoW;

        public DevolucionSustanciasServicio(DbContext entities)
        {
            _UoW = new GenericUnitOfWork(entities);

        }
        public DevolucionSustanciasServicio(IGenericUnitOfWork UoW)
        {
            _UoW = UoW;
        }

        public DevolucionSustanciaVM Agregar(DevolucionSustanciaVM entity)
        {
            DevolucionSustanciaVM result = null;


            AlmacenSustanciaVM lote = new AlmacenSustanciaVM();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                lote = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>(db)
                    .EncontrarPor(w => w.idAlmacen == entity.idAlmacen).FirstOrDefault();
            }

            var tmp = AutoMapperGenericsHelper<AlmacenSustanciaVM, DevolucionSustanciaVM>.ConvertToDBEntity(lote);

            tmp.Existencia = entity.Existencia;
            tmp.IntegrarAlmacen = entity.IntegrarAlmacen;

            tmp.tSustanciaMaterial = null;
            tmp.cConservacion = null;
            tmp.cDisolvente = null;
            tmp.cProveedor = null;
            tmp.cTipoEnvase = null;
            tmp.cTipoSal = null;
            tmp.cUbicacion = null;
            tmp.cUMCantidadEnvase = null;
            tmp.cUMConcentracion = null;

            entity = tmp;

            entity.FG = DateTime.Now;
            entity.UG= HttpContext.Current.User.Identity.GetUserId();
            entity.ST = true;

            entity.UM = null;
            entity.FM = null;
            entity.UB = null;
            entity.FB = null;

            entity.CantidadIngresa = entity.Existencia;
            

            tDevolucionSustancia convert = AutoMapperGenericsHelper<DevolucionSustanciaVM, tDevolucionSustancia>.ConvertToDBEntity(entity);

            _UoW.Repository<tDevolucionSustancia>().Agregar(convert);

            /*Agrega un movimiento al almacen*/

            //if (entity.IntegrarAlmacen)
            //{
            //    tAlmacenSustancia almacen = AutoMapperGenericsHelper<tDevolucionSustancia, tAlmacenSustancia>.ConvertToDBEntity(convert);


            //    tAlmacenMovimiento mov = new tAlmacenMovimiento();

            //    mov.Cantidad = almacen.CantidadIngresa;
            //    mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Devolucion;

            //    mov.FG = DateTime.Now;
            //    mov.UG = HttpContext.Current.User.Identity.GetUserId();
            //    mov.ST = true;

            //    almacen.tAlmacenMovimiento.Add(mov);

            //    _UoW.Repository<tAlmacenSustancia>().Agregar(almacen);
            //}

            /* Se actualiza la existencia en el almacen del lote seleccionado*/
            if (entity.IntegrarAlmacen)
            {
                tAlmacen almacen = _UoW.Repository<tAlmacen>().ObtenerPorId(convert.idAlmacen);


                almacen.Existencia += convert.Existencia;

                tAlmacenMovimiento mov = new tAlmacenMovimiento();

                mov.Cantidad = almacen.CantidadIngresa;
                mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Devolucion;

                mov.FG = DateTime.Now;
                mov.UG = HttpContext.Current.User.Identity.GetUserId();
                mov.ST = true;

                almacen.tAlmacenMovimiento.Add(mov);

                _UoW.Repository<tAlmacen>().Editar(almacen);
            }

            result = AutoMapperGenericsHelper<tDevolucionSustancia, DevolucionSustanciaVM>.ConvertToDBEntity(convert);

            return result;
        }

        public int Contar(Expression<Func<SustanciaVM, bool>> where)
        {
            int result = 0;

            //comentar en caso de obtener tambien los registros inactivos
            Expression<Func<SustanciaVM, bool>> Where2 = w => w.ST == true;
            where = PredicateBuilder.And(Where2, where);

            Expression<Func<tDetalleSustacia, bool>> domainPredicate = AutoMapperGenericsHelper<Expression<Func<SustanciaVM, bool>>, Expression<Func<tDetalleSustacia, bool>>>.ConvertToDBEntity(where);

            _UoW.Repository<tDetalleSustacia>().Contar(domainPredicate);

            return result;
        }

        public void Editar(DevolucionSustanciaVM entity,int id)
        {
            tDevolucionSustancia tmp=null;
            using (CndAlmacenDopajeEntities bd=new CndAlmacenDopajeEntities())
            {
                tmp = new RepositoryGenerico<tDevolucionSustancia>(bd).ObtenerPorId(id);
            }
                
            //DevolucionSustanciaVM tmp = AutoMapperGenericsHelper<tDevolucionSustancia, DevolucionSustanciaVM>.ConvertToDBEntity(entity_db);

            entity.FB = tmp.FB;
            entity.UB = tmp.UB;

            entity.FG = tmp.FG;
            entity.UG = tmp.UG;
            entity.ST = tmp.ST;

            entity.FM = DateTime.Now;
            entity.UM = HttpContext.Current.User.Identity.GetUserId();

            tDevolucionSustancia convert = AutoMapperGenericsHelper<DevolucionSustanciaVM, tDevolucionSustancia>.ConvertToDBEntity(entity);


            _UoW.Repository<tDevolucionSustancia>().Editar(convert);
        }

        public void Eliminar(object id)
        {

            //tDevolucionSustancia entity_db = new RepositoryGenerico<tDevolucionSustancia>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);
            tDevolucionSustancia entity_db=_UoW.Repository<tDevolucionSustancia>().ObtenerPorId(id);

            entity_db.FB = DateTime.Now;
            entity_db.UB = HttpContext.Current.User.Identity.GetUserId();
            entity_db.ST = false;
            
            _UoW.Repository<tDevolucionSustancia>().Eliminar(entity_db);
           
        }

        public List<SustanciaVM> EncontrarPor(ParametrosQuery<SustanciaVM> parametrosDeQuery)
        {
            List<SustanciaVM> result = null;

            Expression<Func<SustanciaVM, bool>> Where2 = w => w.ST == true;
            parametrosDeQuery.Where = PredicateBuilder.And(Where2, parametrosDeQuery.Where);

            ParametrosQuery<tDetalleSustacia> domainPredicate = AutoMapperGenericsHelper<ParametrosQuery<SustanciaVM>, ParametrosQuery<tDetalleSustacia>>.ConvertToDBEntity(parametrosDeQuery);

            IEnumerable<tDetalleSustacia> entitys = _UoW.Repository<tDetalleSustacia>().EncontrarPor(domainPredicate.Where);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleSustacia>, List<SustanciaVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public SustanciaVM Obtener(Expression<Func<tDetalleSustacia, bool>> predicate)
        {
            SustanciaVM result = null;

            tDetalleSustacia entity = _UoW.Repository<tDetalleSustacia>().Obtener(predicate);

            result = AutoMapperGenericsHelper<tDetalleSustacia, SustanciaVM>.ConvertToDBEntity(entity);

            return result;
        }

        public DevolucionSustanciaVM ObtenerPorId(object id)
        {
            DevolucionSustanciaVM result = null;

            tDevolucionSustancia entity = _UoW.Repository<tDevolucionSustancia>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<tDevolucionSustancia, DevolucionSustanciaVM>.ConvertToDBEntity(entity);

            //using (LeerCatalogosServicio<SustanciaVM, tDetalleSustacia> db = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>())
            //{
            //    result.tSustanciaMaterial = db.ObtenerPorId(result.idSustanciaMaterial);
            //}

            AlmacenSustanciaVM lote = new AlmacenSustanciaVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                if (result.idAlmacen > 0)
                {
                    lote = new LeerCatalogosServicio<AlmacenSustanciaVM, tAlmacenSustancia>(db)
                            .EncontrarPor(w => w.idAlmacen == result.idAlmacen).FirstOrDefault();
                }
            }

            if (lote!=null)
            {

                result.cConservacion = lote.cConservacion;
                result.cDisolvente = lote.cDisolvente;
                result.cProveedor = lote.cProveedor;
                result.cTipoEnvase = lote.cTipoEnvase;
                result.cTipoSal = lote.cTipoSal;
                result.cUbicacion = lote.cUbicacion;
                result.cUMCantidadEnvase = lote.cUMCantidadEnvase;
                result.cUMConcentracion = lote.cUMConcentracion;
            }

            return result;
        }

        public List<SustanciaVM> ObtenerTodos(Expression<Func<tDetalleSustacia, bool>> predicate = null)
        {
            List<SustanciaVM> result = null;

            predicate=predicate.And(w => w.ST == true && w.tDevolucion.Count > 0);

            IEnumerable<tDetalleSustacia> entitys = _UoW.Repository<tDetalleSustacia>().ObtenerTodos(predicate);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleSustacia>, List<SustanciaVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public BaseListaCatalogosVM<SustanciaVM>  ObtenerPaginado(ParametrosQuery<tDetalleSustacia> parametrosDeQuery)
        {
            BaseListaCatalogosVM<SustanciaVM> result = new BaseListaCatalogosVM<SustanciaVM>();

            if (parametrosDeQuery.Where != null)
            {
                
                parametrosDeQuery.Where = parametrosDeQuery.Where.And(w => w.ST == true && w.tDevolucion.Count > 0);
            }
            else
            {
                parametrosDeQuery.Where= w => w.ST == true && w.tDevolucion.Count > 0;
            }

            Paginacion<tDetalleSustacia> paginado = _UoW.Repository<tDetalleSustacia>().ObtenerPaginado(parametrosDeQuery);


            result.Paginacion = AutoMapperGenericsHelper<Paginacion<tDetalleSustacia>, Paginacion<SustanciaVM>>.ConvertToDBEntity(paginado);

            result.Paginador = Mapper.Map<Paginacion<SustanciaVM>, PaginadorViewModels>(result.Paginacion);

            return result;
        }

        public BaseListaAlmacenVM<SustanciaVM,DevolucionSustanciaVM> ObtenerExistencias(int idSustanciaMaterial, Expression<Func<tDevolucionSustancia, bool>> predicateExistencias = null)
        {
            BaseListaAlmacenVM<SustanciaVM, DevolucionSustanciaVM> result = new BaseListaAlmacenVM<SustanciaVM, DevolucionSustanciaVM>();

            predicateExistencias=predicateExistencias.And(w => w.ST == true);

            IEnumerable<tDevolucionSustancia> entitys = _UoW.Repository<tDevolucionSustancia>().ObtenerTodos(predicateExistencias);

            result.Lista = AutoMapperGenericsHelper<IEnumerable<tDevolucionSustancia>, List<DevolucionSustanciaVM>>.ConvertToDBEntity(entitys);

            using (LeerCatalogosServicio<SustanciaVM, tDetalleSustacia> db = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>())
            {
                result.Material = db.ObtenerPorId(idSustanciaMaterial);
            }

            return result;
        }
        public int Existencias(int idSustanciaMaterial, Expression<Func<tDevolucionSustancia, bool>> predicateExistencias = null)
        {
            int result = 0;

            predicateExistencias = predicateExistencias.And(w => w.ST == true);

            result = _UoW.Repository<tDevolucionSustancia>().EncontrarPor(predicateExistencias).Sum(su => su.Existencia);

            return result;
        }
        private static void FiltrosYSoloActivos(Expression<Func<tDetalleSustacia, bool>> parametros)
        { 

            Expression<Func<SustanciaVM, bool>> Where2 = w => w.ST == true;

            var lambda = Where2;

            if (parametros != null)
            {
                var domainPredicate = AutoMapperGenericsHelper<Expression<Func<tDetalleSustacia, bool>>, Expression<Func<SustanciaVM, bool>>>.ConvertToDBEntity(parametros);
                lambda = PredicateBuilder.And(Where2, domainPredicate);
            }

            parametros = AutoMapperGenericsHelper<Expression<Func<SustanciaVM, bool>>, Expression<Func<tDetalleSustacia, bool>>>.ConvertToDBEntity(lambda);
        }

        public ResultSave Save()
        {
           return _UoW.SaveChanges();
        }

        public int GenNumDeLoteInterno(int idSustanciaMaterial)
        {
            int result = 1;

            tDevolucionSustancia material= _UoW.Repository<tDevolucionSustancia>()
                .EncontrarPor(w=> w.idSustanciaMaterial== idSustanciaMaterial)
                .OrderByDescending(o => o.NumDeLoteInterno).Take(1).FirstOrDefault();

            if (material!=null)
            {
                result = material.NumDeLoteInterno+1;
            }

            return result;
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