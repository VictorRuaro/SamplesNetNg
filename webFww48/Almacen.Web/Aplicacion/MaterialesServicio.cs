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
    public class MaterialesServicio : IDisposable
    {
        private readonly IGenericUnitOfWork _UoW;

        public MaterialesServicio(DbContext entities)
        {
            _UoW = new GenericUnitOfWork(entities);

        }
        public MaterialesServicio(IGenericUnitOfWork UoW)
        {
            _UoW = UoW;
        }

        public AlmacenMaterialVM Agregar(AlmacenMaterialVM entity)
        {
            AlmacenMaterialVM result = null;

            entity.FG = DateTime.Now;
            entity.UG= HttpContext.Current.User.Identity.GetUserId();
            entity.ST = true;

            entity.CantidadIngresa = entity.Existencia;

            tAlmacenMaterial convert = AutoMapperGenericsHelper<AlmacenMaterialVM, tAlmacenMaterial>.ConvertToDBEntity(entity);

            

            tAlmacenMovimiento mov = new tAlmacenMovimiento();

            mov.Cantidad = convert.CantidadIngresa;
            mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Entrada;

            mov.FG = DateTime.Now;
            mov.UG = HttpContext.Current.User.Identity.GetUserId();
            mov.ST = true;

            convert.tAlmacenMovimiento.Add(mov);

            //_UoW.Repository<tAlmacenMovimiento>().Agregar(mov);

            _UoW.Repository<tAlmacenMaterial>().Agregar(convert);

            result = AutoMapperGenericsHelper<tAlmacenMaterial, AlmacenMaterialVM>.ConvertToDBEntity(convert);

            return result;
        }

        public int Contar(Expression<Func<MaterialVM, bool>> where)
        {
            int result = 0;

            //comentar en caso de obtener tambien los registros inactivos
            Expression<Func<MaterialVM, bool>> Where2 = w => w.ST == true;
            where = PredicateBuilder.And(Where2, where);

            Expression<Func<tDetalleMaterial, bool>> domainPredicate = AutoMapperGenericsHelper<Expression<Func<MaterialVM, bool>>, Expression<Func<tDetalleMaterial, bool>>>.ConvertToDBEntity(where);

            _UoW.Repository<tDetalleMaterial>().Contar(domainPredicate);

            return result;
        }

        public void Editar(AlmacenMaterialVM entity,int id)
        {

            tAlmacenMaterial tmp = null;
            using (CndAlmacenDopajeEntities bd = new CndAlmacenDopajeEntities())
            {
                tmp = new RepositoryGenerico<tAlmacenMaterial>(bd).ObtenerPorId(id);
            }

            //AlmacenMaterialVM tmp = AutoMapperGenericsHelper<tAlmacenMaterial, AlmacenMaterialVM>.ConvertToDBEntity(entity_db);

            entity.FB = tmp.FB;
            entity.UB = tmp.UB;

            entity.FG = tmp.FG;
            entity.UG = tmp.UG;
            entity.ST = tmp.ST;

            entity.FM = DateTime.Now;
            entity.UM = HttpContext.Current.User.Identity.GetUserId();

            tAlmacenMaterial convert = AutoMapperGenericsHelper<AlmacenMaterialVM, tAlmacenMaterial>.ConvertToDBEntity(entity);

            _UoW.Repository<tAlmacenMaterial>().Editar(convert);
        }

        public void Eliminar(object id)
        {

            //tAlmacenMaterial entity_db = new RepositoryGenerico<tAlmacenMaterial>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);
            tAlmacenMaterial entity_db = _UoW.Repository<tAlmacenMaterial>().ObtenerPorId(id);

            entity_db.FB = DateTime.Now;
            entity_db.UB = HttpContext.Current.User.Identity.GetUserId();
            entity_db.ST = false;

            _UoW.Repository<tAlmacenMaterial>().Eliminar(entity_db);
           
        }

        public List<MaterialVM> EncontrarPor(ParametrosQuery<MaterialVM> parametrosDeQuery)
        {
            List<MaterialVM> result = null;

            Expression<Func<MaterialVM, bool>> Where2 = w => w.ST == true;
            parametrosDeQuery.Where = PredicateBuilder.And(Where2, parametrosDeQuery.Where);

            ParametrosQuery<tDetalleMaterial> domainPredicate = AutoMapperGenericsHelper<ParametrosQuery<MaterialVM>, ParametrosQuery<tDetalleMaterial>>.ConvertToDBEntity(parametrosDeQuery);

            IEnumerable<tDetalleMaterial> entitys = _UoW.Repository<tDetalleMaterial>().EncontrarPor(domainPredicate.Where);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleMaterial>, List<MaterialVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public MaterialVM Obtener(Expression<Func<tDetalleMaterial, bool>> predicate)
        {
            MaterialVM result = null;

            tDetalleMaterial entity = _UoW.Repository<tDetalleMaterial>().Obtener(predicate);

            result = AutoMapperGenericsHelper<tDetalleMaterial, MaterialVM>.ConvertToDBEntity(entity);

            return result;
        }

        public AlmacenMaterialVM ObtenerPorId(object id)
        {
            AlmacenMaterialVM result = null;

            tAlmacenMaterial entity = _UoW.Repository<tAlmacenMaterial>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<tAlmacenMaterial, AlmacenMaterialVM>.ConvertToDBEntity(entity);

            //using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            //{
            //    result.tSustanciaMaterial = db.ObtenerPorId(result.idSustanciaMaterial);
            //}

            return result;
        }

        public List<MaterialVM> ObtenerTodos(Expression<Func<tDetalleMaterial, bool>> predicate = null)
        {
            List<MaterialVM> result = null;

            predicate=predicate.And(w => w.ST == true 
                                    && w.tAlmacen.Count > 0 
                                    && w.tAlmacen.Where(a => a.ST == true).Any());

            IEnumerable<tDetalleMaterial> entitys = _UoW.Repository<tDetalleMaterial>().ObtenerTodos(predicate);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleMaterial>, List<MaterialVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public BaseListaCatalogosVM<MaterialVM>  ObtenerPaginado(ParametrosQuery<tDetalleMaterial> parametrosDeQuery)
        {
            BaseListaCatalogosVM<MaterialVM> result = new BaseListaCatalogosVM<MaterialVM>();

            if (parametrosDeQuery.Where != null)
            {
                parametrosDeQuery.Where = parametrosDeQuery.Where.And(w => w.ST == true 
                                                                        && w.tAlmacen.Count > 0 
                                                                        && w.tAlmacen.Where(a => a.ST == true).Any());
            }
            else
            {
                parametrosDeQuery.Where= w => w.ST == true 
                                        && w.tAlmacen.Count > 0 
                                        && w.tAlmacen.Where(a => a.ST == true).Any();
            }

            Paginacion<tDetalleMaterial> paginado = _UoW.Repository<tDetalleMaterial>().ObtenerPaginado(parametrosDeQuery);


            result.Paginacion = AutoMapperGenericsHelper<Paginacion<tDetalleMaterial>, Paginacion<MaterialVM>>.ConvertToDBEntity(paginado);

            result.Paginador = Mapper.Map<Paginacion<MaterialVM>, PaginadorViewModels>(result.Paginacion);

            return result;
        }

        public BaseListaAlmacenVM<MaterialVM, AlmacenMaterialVM> ObtenerExistencias(int idSustanciaMaterial, Expression<Func<tAlmacenMaterial, bool>> predicateExistencias = null)
        {
            BaseListaAlmacenVM<MaterialVM, AlmacenMaterialVM> result = new BaseListaAlmacenVM<MaterialVM, AlmacenMaterialVM>();

            predicateExistencias=predicateExistencias.And(w => w.ST == true);

            IEnumerable<tAlmacenMaterial> entitys = _UoW.Repository<tAlmacenMaterial>().ObtenerTodos(predicateExistencias);

            result.Lista = AutoMapperGenericsHelper<IEnumerable<tAlmacenMaterial>, List<AlmacenMaterialVM>>.ConvertToDBEntity(entitys);

            using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            {
                result.Material = db.ObtenerPorId(idSustanciaMaterial);
            }

            return result;
        }

        public int Existencias(int idSustanciaMaterial, Expression<Func<tAlmacenMaterial, bool>> predicateExistencias = null)
        {
            int result = 0;

            predicateExistencias = predicateExistencias.And(w => w.ST == true);

            result = _UoW.Repository<tAlmacenMaterial>().EncontrarPor(predicateExistencias).Sum(su=>su.Existencia);

            return result;
        }
        private static void FiltrosYSoloActivos(Expression<Func<tDetalleMaterial, bool>> parametros)
        { 

            Expression<Func<MaterialVM, bool>> Where2 = w => w.ST == true;

            var lambda = Where2;

            if (parametros != null)
            {
                var domainPredicate = AutoMapperGenericsHelper<Expression<Func<tDetalleMaterial, bool>>, Expression<Func<MaterialVM, bool>>>.ConvertToDBEntity(parametros);
                lambda = PredicateBuilder.And(Where2, domainPredicate);
            }

            parametros = AutoMapperGenericsHelper<Expression<Func<MaterialVM, bool>>, Expression<Func<tDetalleMaterial, bool>>>.ConvertToDBEntity(lambda);
        }

        public ResultSave Save()
        {
           return _UoW.SaveChanges();
        }

        public int GenNumDeLoteInterno(int idSustanciaMaterial)
        {
            int result = 1;

            tAlmacenMaterial material= _UoW.Repository<tAlmacenMaterial>()
                .EncontrarPor(w=> w.idSustanciaMaterial== idSustanciaMaterial)
                .OrderByDescending(o => o.NumDeLoteInterno).Take(1).FirstOrDefault();

            if (material!=null)
            {
                result = material.NumDeLoteInterno+1;
            }

            return result;
        }

        //private static Expression<Func<tDetalleMaterial, TType>> MapDomainModelExpression<TType>(Expression<Func<MaterialVM, TType>> expression)
        //{
        //    return Mapper.Map<Expression<Func<MaterialVM, TType>>, Expression<Func<tDetalleMaterial, TType>>>(expression);
        //}

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