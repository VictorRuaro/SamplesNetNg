using System;
using System.Collections.Generic;
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
    public class LeerCatalogosServicio<TModel, Tentity> : ILeerCatalogosServicio<TModel, Tentity> where TModel : EntityBase where Tentity : class 
    {
        private readonly IRepository<Tentity> _Repo;

        private readonly DbContext _entities;
        public LeerCatalogosServicio()
        {

            _entities = new CndAlmacenDopajeEntities();

            _Repo = new RepositoryGenerico<Tentity>(_entities);

            //var dest = Mapper.Map<Source<TModel>, Destination<Tentity>>(source);

            //cfg.CreateMap(typeof(TModel), typeof(Tentity)).IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
            //cfg.CreateMap(typeof(Tentity), typeof(TModel)).IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

        }

        public static Tentity ConvertToDBEntity(TModel model)
        {
            return Mapper.Map<TModel, Tentity>(model);
            
        }

        public LeerCatalogosServicio(DbContext entities)
        {
            if (entities == null) throw new ArgumentNullException("entities is null");

            _Repo = new RepositoryGenerico<Tentity>(entities);

        }

        public int Contar(Expression<Func<TModel, bool>> where)
        {
            int result = 0;

            //comentar en caso de obtener tambien los registros inactivos
            Expression<Func<TModel, bool>> Where2 = w => w.ST == true;
            where = PredicateBuilder.And(Where2, where);

            Expression<Func<Tentity, bool>> domainPredicate = GenericsHelper<Expression<Func<TModel, bool>>, Expression<Func<Tentity, bool>>>.ConvertToDBEntity(where);

            _Repo.Contar(domainPredicate);

            return result;
        }

        public List<TModel> EncontrarPor(Expression<Func<TModel, bool>> predicate)
        {
            List<TModel> result = null;

            Expression<Func<TModel, bool>> Where2 = w => w.ST == true;
            predicate = PredicateBuilder.And(Where2, predicate);

            Expression<Func<Tentity, bool>> domainPredicate = GenericsHelper<Expression<Func<TModel, bool>>, Expression<Func<Tentity, bool>>>.ConvertToDBEntity(predicate);

            IEnumerable<Tentity> entitys = _Repo.EncontrarPor(domainPredicate);

            result = GenericsHelper<IEnumerable<Tentity>, List<TModel>>.ConvertToDBEntity(entitys);

            return result;
        }

        public TModel Obtener(Expression<Func<Tentity, bool>> predicate)
        {
            TModel result = null;

            Tentity entity = _Repo.Obtener(predicate);

            result = GenericsHelper<Tentity, TModel>.ConvertToDBEntity(entity);

            return result;
        }

        public TModel ObtenerPorId(object id)
        {
            TModel result = null;

            Tentity entity = _Repo.ObtenerPorId(id);

            result = GenericsHelper<Tentity, TModel>.ConvertToDBEntity(entity);

            return result;
        }

        public List<TModel> ObtenerTodos(Expression<Func<Tentity, bool>> predicate = null)
        {
            List<TModel> result = null;

            predicate=FiltrosYSoloActivos(predicate);

            IEnumerable<Tentity> entitys = _Repo.ObtenerTodos(predicate);

            result = GenericsHelper<IEnumerable<Tentity>, List<TModel>>.ConvertToDBEntity(entitys);

            return result;
        }

        public List<TModel> ObtenerPaginado(ParametrosQuery<Tentity> parametrosDeQuery)
        {
            List<TModel> result = new List<TModel>();

            parametrosDeQuery.Where=FiltrosYSoloActivos(parametrosDeQuery.Where);

            IEnumerable<Tentity> paginado = _Repo.EncontrarPor(parametrosDeQuery.Where);

            result = GenericsHelper<IEnumerable<Tentity>, List<TModel>>.ConvertToDBEntity(paginado);

            return result;
        }

        private Expression<Func<Tentity, bool>> FiltrosYSoloActivos(Expression<Func<Tentity, bool>> parametros)
        {
            Expression<Func<Tentity, bool>> result=null;

            Expression<Func<TModel, bool>> Where2 = w => w.ST == true;

            var lambda = Where2;
            if (parametros != null)
            {
                var domainPredicate = GenericsHelper<Expression<Func<Tentity, bool>>, Expression<Func<TModel, bool>>>.ConvertToDBEntity(parametros);
                lambda = PredicateBuilder.And(Where2, domainPredicate);
            }

            result = GenericsHelper<Expression<Func<TModel, bool>>, Expression<Func<Tentity, bool>>>.ConvertToDBEntity(lambda);

            return result;
        }

        private static Expression<Func<Tentity, TType>> MapDomainModelExpression<TType>(Expression<Func<TModel, TType>> expression)
        {
            return Mapper.Map<Expression<Func<TModel, TType>>, Expression<Func<Tentity, TType>>>(expression);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _Repo.Dispose();
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

    public static class GenericsHelper<TSource, TDestination>
    {
        public static TDestination ConvertToDBEntity(TSource model)
        {

            //var val = new Source<TSource>();

            //val.Value = model;

            //var dest = Mapper.Map<Source<TSource>, Destination<TDestination>>(val);

            return Mapper.Map<TSource, TDestination>(model, cfg=> cfg.ConfigureMap().IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter() );
        }

    }





}