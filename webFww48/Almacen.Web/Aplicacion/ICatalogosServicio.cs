using Almacen.Infraestructura;
using Almacen.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Almacen.Web.Applicacion
{
    public interface ICatalogosServicio<TModel, TEntity> : IDisposable where TModel : EntityBase where TEntity : class
    {
        List<TModel> ObtenerTodos(Expression<Func<TEntity, bool>> predicate = null);
        BaseListaCatalogosVM<TModel> ObtenerPaginado(ParametrosQuery<TEntity> parametrosDeQuery);
        TModel Obtener(Expression<Func<TEntity, bool>> predicate);
        TModel ObtenerPorId(object id);
        TModel Agregar(TModel entity);
        void Editar(TModel entity, int id);
        void Eliminar(object id);
        int Contar(Expression<Func<TModel, bool>> where);
        List<TModel> EncontrarPor(Expression<Func<TModel, bool>> predicate = null);

        ResultSave Save();
    }



}