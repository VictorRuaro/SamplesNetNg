using Almacen.Infraestructura;
using Almacen.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Almacen.Web.Applicacion
{
    public interface ILeerCatalogosServicio<TModel, TEntity> : IDisposable where TModel : EntityBase where TEntity : class
    {
        List<TModel> ObtenerTodos(Expression<Func<TEntity, bool>> predicate = null);
        List<TModel> ObtenerPaginado(ParametrosQuery<TEntity> parametrosDeQuery);
        TModel Obtener(Expression<Func<TEntity, bool>> predicate);
        TModel ObtenerPorId(object id);
        int Contar(Expression<Func<TModel, bool>> where);
        List<TModel> EncontrarPor(Expression<Func<TModel, bool>> predicate);
    }



}