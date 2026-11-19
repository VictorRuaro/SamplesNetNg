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
using Almacen.Data;
using Almacen.Web.Aplicacion;
using Almacen.Web.ViewModels.Catalogos;

namespace Almacen.Web.Applicacion
{
    public class AlertasServicio<TModel, TEntity> : ICatalogosServicio<TModel, TEntity> where TModel : EntityBase where TEntity : class 
    {
        private readonly IGenericUnitOfWork _UoW;

        public AlertasServicio(DbContext entities)
        {
            _UoW = new GenericUnitOfWork(entities);

        }
        public AlertasServicio(IGenericUnitOfWork UoW)
        {
            _UoW = UoW;
        }

        public TModel Agregar(TModel entity)
        {
            TModel result = null;

            entity.FG = DateTime.Now;
            entity.UG= HttpContext.Current.User.Identity.GetUserId();
            entity.ST = true;

            TEntity convert = AutoMapperGenericsHelper<TModel, TEntity>.ConvertToDBEntity(entity);

            _UoW.Repository<TEntity>().Agregar(convert);

            result = AutoMapperGenericsHelper<TEntity, TModel>.ConvertToDBEntity(convert);

            return result;
        }

        public int Contar(Expression<Func<TModel, bool>> where)
        {
            int result = 0;

            //comentar en caso de obtener tambien los registros inactivos
            Expression<Func<TModel, bool>> Where2 = w => w.ST == true;
            where = PredicateBuilder.And(Where2, where);

            Expression<Func<TEntity, bool>> domainPredicate = AutoMapperGenericsHelper<Expression<Func<TModel, bool>>, Expression<Func<TEntity, bool>>>.ConvertToDBEntity(where);

            _UoW.Repository<TEntity>().Contar(domainPredicate);

            return result;
        }

        

        public void Editar(TModel entity,int id)
        {

            TEntity entity_db = new RepositoryGenerico<TEntity>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);

            TModel tmp = AutoMapperGenericsHelper<TEntity,TModel>.ConvertToDBEntity(entity_db);

            entity.FG = tmp.FG;
            entity.UG = tmp.UG;
            entity.ST = tmp.ST;

            entity.FM = DateTime.Now;
            entity.UM = HttpContext.Current.User.Identity.GetUserId();

            TEntity convert = AutoMapperGenericsHelper<TModel, TEntity>.ConvertToDBEntity(entity);

            _UoW.Repository<TEntity>().Editar(convert);
        }

        public void Eliminar(object id)
        {

            TEntity entity_db = new RepositoryGenerico<TEntity>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);

            TModel entity = AutoMapperGenericsHelper<TEntity, TModel>.ConvertToDBEntity(entity_db);

            entity.FB = DateTime.Now;
            entity.UB = HttpContext.Current.User.Identity.GetUserId();
            entity.ST = false;

            TEntity convert = AutoMapperGenericsHelper<TModel, TEntity>.ConvertToDBEntity(entity);

            _UoW.Repository<TEntity>().Eliminar(convert);
           
        }

        public List<TModel> EncontrarPor(Expression<Func<TModel, bool>> predicate)
        {
            List<TModel> result = null;

            Expression<Func<TModel, bool>> Where2 = w => w.ST == true;
            predicate = PredicateBuilder.And(Where2, predicate);

            Expression<Func<TEntity, bool>> domainPredicate = AutoMapperGenericsHelper<Expression<Func<TModel, bool>>, Expression<Func<TEntity, bool>>>.ConvertToDBEntity(predicate);

            IEnumerable<TEntity> entitys = _UoW.Repository<TEntity>().EncontrarPor(domainPredicate).ToList();

            result = AutoMapperGenericsHelper<IEnumerable<TEntity>, List<TModel>>.ConvertToDBEntity(entitys);

            return result;
        }

        public TModel Obtener(Expression<Func<TEntity, bool>> predicate)
        {
            TModel result = null;

            TEntity entity = _UoW.Repository<TEntity>().Obtener(predicate);

            result = AutoMapperGenericsHelper<TEntity, TModel>.ConvertToDBEntity(entity);

            return result;
        }

        public TModel ObtenerPorId(object id)
        {
            TModel result = null;

            TEntity entity = _UoW.Repository<TEntity>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<TEntity, TModel>.ConvertToDBEntity(entity);

            return result;
        }

        public List<TModel> ObtenerTodos(Expression<Func<TEntity, bool>> predicate = null)
        {
            List<TModel> result = null;

            predicate=FiltrosYSoloActivos(predicate);

            IEnumerable<TEntity> entitys = _UoW.Repository<TEntity>().ObtenerTodos(predicate);

            result = AutoMapperGenericsHelper<IEnumerable<TEntity>, List<TModel>>.ConvertToDBEntity(entitys);

            return result;
        }

        public bool AgregarSustanciaMaterial(AlertaXSustanciaMaterialVM model)
        {

            model.FG = DateTime.Now;
            model.UG = HttpContext.Current.User.Identity.GetUserId();
            model.ST = true;

            tAlertaXSustanciaMaterial convert = AutoMapperGenericsHelper<AlertaXSustanciaMaterialVM, tAlertaXSustanciaMaterial>.ConvertToDBEntity(model);

            _UoW.Repository<tAlertaXSustanciaMaterial>().Agregar(convert);

            return true;
        }

        public void EditarSustanciaMaterial(AlertaXSustanciaMaterialVM model, int id)
        {
            tAlertaXSustanciaMaterial entity_db = new RepositoryGenerico<tAlertaXSustanciaMaterial>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);

            AlertaXSustanciaMaterialVM tmp = AutoMapperGenericsHelper<tAlertaXSustanciaMaterial, AlertaXSustanciaMaterialVM>.ConvertToDBEntity(entity_db);

            model.FG = tmp.FG;
            model.UG = tmp.UG;
            model.ST = tmp.ST;

            model.FM = DateTime.Now;
            model.UM = HttpContext.Current.User.Identity.GetUserId();

            tAlertaXSustanciaMaterial convert = AutoMapperGenericsHelper<AlertaXSustanciaMaterialVM,tAlertaXSustanciaMaterial>.ConvertToDBEntity(model);

            _UoW.Repository<tAlertaXSustanciaMaterial>().Editar(convert);

        }

        public AlertaXSustanciaMaterialVM EditarSustanciaMaterial(int ValorEditado,int Valor100referenciaEdit, int id)
        {
            AlertaXSustanciaMaterialVM result = new AlertaXSustanciaMaterialVM();

            var entity_db = _UoW.Repository<tAlertaXSustanciaMaterial>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<tAlertaXSustanciaMaterial, AlertaXSustanciaMaterialVM>.ConvertToDBEntity(entity_db);



            entity_db.Valor = ValorEditado;
            entity_db.Valor100referencia = Valor100referenciaEdit;

            entity_db.FM = DateTime.Now;
            entity_db.UM = HttpContext.Current.User.Identity.GetUserId();

            _UoW.Repository<tAlertaXSustanciaMaterial>().Editar(entity_db);


            return result;
        }

        public AlertaXSustanciaMaterialVM ObtenerPorIdSustanciaMaterial(int? id)
        {
            AlertaXSustanciaMaterialVM result = null;

            tAlertaXSustanciaMaterial entity = _UoW.Repository<tAlertaXSustanciaMaterial>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<tAlertaXSustanciaMaterial, AlertaXSustanciaMaterialVM>.ConvertToDBEntity(entity);

            return result;
        }

        public void EliminarSustanciaMaterial(object id)
        {
            //tAlertaXSustanciaMaterial entity_db = new RepositoryGenerico<tAlertaXSustanciaMaterial>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);

            //AlertaXSustanciaMaterialVM entity = AutoMapperGenericsHelper<tAlertaXSustanciaMaterial, AlertaXSustanciaMaterialVM>.ConvertToDBEntity(entity_db);

            tAlertaXSustanciaMaterial entity =_UoW.Repository<tAlertaXSustanciaMaterial>().ObtenerPorId(id);

            entity.FB = DateTime.Now;
            entity.UB = HttpContext.Current.User.Identity.GetUserId();
            entity.ST = false;

            //tAlertaXSustanciaMaterial convert = AutoMapperGenericsHelper<AlertaXSustanciaMaterialVM, tAlertaXSustanciaMaterial>.ConvertToDBEntity(entity);

            _UoW.Repository<tAlertaXSustanciaMaterial>().Eliminar(entity);
        }

        public List<AlertaXSustanciaMaterialVM> ObtenerTodosSustanciaMaterial(Expression<Func<tAlertaXSustanciaMaterial, bool>> predicate = null)
        {
            List<AlertaXSustanciaMaterialVM> result = null;

            Expression<Func<tAlertaXSustanciaMaterial, bool>> Where2 = w => w.ST == true;
            predicate = PredicateBuilder.And(Where2, predicate);

            IEnumerable<tAlertaXSustanciaMaterial> entitys = _UoW.Repository<tAlertaXSustanciaMaterial>().ObtenerTodos(predicate);

            result = AutoMapperGenericsHelper<IEnumerable<tAlertaXSustanciaMaterial>, List<AlertaXSustanciaMaterialVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public BaseListaCatalogosVM<TModel> ObtenerPaginado(ParametrosQuery<TEntity> parametrosDeQuery)
        {
            BaseListaCatalogosVM<TModel> result = new BaseListaCatalogosVM<TModel>();

            parametrosDeQuery.Where=FiltrosYSoloActivos(parametrosDeQuery.Where);

            Paginacion<TEntity> paginado = _UoW.Repository<TEntity>().ObtenerPaginado(parametrosDeQuery);

            result.Paginacion = AutoMapperGenericsHelper<Paginacion<TEntity>, Paginacion<TModel>>.ConvertToDBEntity(paginado);

            result.Paginador = Mapper.Map<Paginacion<TModel>, PaginadorViewModels>(result.Paginacion);

            return result;
        }

        private Expression<Func<TEntity, bool>> FiltrosYSoloActivos(Expression<Func<TEntity, bool>> parametros)
        {
            Expression<Func<TEntity, bool>> result=null;

            Expression<Func<TModel, bool>> Where2 = w => w.ST == true;

            var lambda = Where2;
            if (parametros != null)
            {
                var domainPredicate = AutoMapperGenericsHelper<Expression<Func<TEntity, bool>>, Expression<Func<TModel, bool>>>.ConvertToDBEntity(parametros);
                lambda = PredicateBuilder.And(Where2, domainPredicate);
            }

            result = AutoMapperGenericsHelper<Expression<Func<TModel, bool>>, Expression<Func<TEntity, bool>>>.ConvertToDBEntity(lambda);

            return result;
        }

        public ResultSave Save()
        {
           return _UoW.SaveChanges();
        }

        //private static Expression<Func<TEntity, TType>> MapDomainModelExpression<TType>(Expression<Func<TModel, TType>> expression)
        //{
        //    return Mapper.Map<Expression<Func<TModel, TType>>, Expression<Func<TEntity, TType>>>(expression);
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