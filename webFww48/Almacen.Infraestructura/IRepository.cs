namespace Almacen.Infraestructura
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    public interface IRepository<T>: IDisposable where T : class
    {
        List<T> ObtenerTodos(Expression<Func<T, bool>> predicate = null);
        Paginacion<T> ObtenerPaginado(ParametrosQuery<T> parametrosDeQuery);
        T Obtener(Expression<Func<T, bool>> predicate);
        T ObtenerPorId(object id);
        T Agregar(T entity);
        void Editar(T entity);
        void Eliminar(T entity);
        void Eliminar(object id);
        int Contar(Expression<Func<T, bool>> where);
        IQueryable<T> EncontrarPor(Expression<Func<T, bool>> predicate = null);
    }
}
