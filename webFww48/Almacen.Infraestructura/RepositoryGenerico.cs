
namespace Almacen.Infraestructura
{
    using System;
    using System.Data.Entity;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class RepositoryGenerico<T> : IRepository<T> where T : class
    {
        private DbContext entities;
        private DbSet<T> _objectSet;

        public RepositoryGenerico(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            entities = context;
            _objectSet = entities.Set<T>();
        }

        public List<T> ObtenerTodos(Expression<Func<T, bool>> predicate = null)
        {

            if(predicate != null)
            {
                return _objectSet.Where(predicate).AsEnumerable().ToList();
            }

            return _objectSet.AsEnumerable().ToList();
        }

        public Paginacion<T> ObtenerPaginado(ParametrosQuery<T> parametrosDeQuery=null)
        {
            Paginacion<T> result = new Paginacion<T>();


            if (parametrosDeQuery != null)
            {
                var orderByClass = ObtenerOrderBy(parametrosDeQuery);
                Expression<Func<T, bool>> whereTrue = x => true;
                var where = (parametrosDeQuery.Where == null) ? whereTrue : parametrosDeQuery.Where;

                
                if (orderByClass==null)
                {
                    if (parametrosDeQuery.PaginaActual > 0)
                    {
                        result.TotalRegistros = _objectSet.Where(where).Count();
                        if(result.TotalRegistros< parametrosDeQuery.RegistrosPorPagina)
                        {
                            parametrosDeQuery.PaginaActual = 1;
                        }

                        result.Lista = _objectSet.AsQueryable().Where(where).AsEnumerable()
                        .Skip((parametrosDeQuery.PaginaActual - 1) * parametrosDeQuery.RegistrosPorPagina)
                        .Take(parametrosDeQuery.RegistrosPorPagina)
                        .ToList();
                    }
                    else
                    {
                        result.Lista = _objectSet.Where(where).ToList();
                        result.TotalRegistros = result.Lista.Count();
                    }
                }
                else if(orderByClass.IsAscending)
                {
                    if (parametrosDeQuery.PaginaActual > 0)
                    {
                        result.TotalRegistros = _objectSet.Where(where).Count();
                        if (result.TotalRegistros < parametrosDeQuery.RegistrosPorPagina)
                        {
                            parametrosDeQuery.PaginaActual = 1;
                        }
                        result.Lista = _objectSet.Where(where).OrderBy(orderByClass.OrderBy)
                        .Skip((parametrosDeQuery.PaginaActual - 1) * parametrosDeQuery.RegistrosPorPagina)
                        .Take(parametrosDeQuery.RegistrosPorPagina).ToList();
                    }
                    else
                    {
                        result.Lista = _objectSet.Where(where).OrderBy(orderByClass.OrderBy).ToList();
                        result.TotalRegistros = result.Lista.Count();
                    }
                }
                else
                {
                    if (parametrosDeQuery.PaginaActual > 0)
                    {
                        result.TotalRegistros = _objectSet.Where(where).Count();
                        if (result.TotalRegistros < parametrosDeQuery.RegistrosPorPagina)
                        {
                            parametrosDeQuery.PaginaActual = 1;
                        }
                        result.Lista = _objectSet.Where(where).OrderByDescending(orderByClass.OrderBy)
                        .Skip((parametrosDeQuery.PaginaActual - 1) * parametrosDeQuery.RegistrosPorPagina)
                        .Take(parametrosDeQuery.RegistrosPorPagina).ToList();
                        }
                    else
                    {
                        result.Lista = _objectSet.Where(where).OrderByDescending(orderByClass.OrderBy).ToList();
                        result.TotalRegistros = result.Lista.Count();
                    }
                }
            }else
            {
                result.Lista = _objectSet.AsEnumerable();
                result.TotalRegistros = result.Lista.Count();
            }

            result.PaginaActual = parametrosDeQuery.PaginaActual;
            result.RegistrosPorPagina = parametrosDeQuery.RegistrosPorPagina;

            return result;

        }


        public T Obtener(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).First();
        }

        public T ObtenerPorId(object id)
        {
            return _objectSet.Find(id);
        }

        public T Agregar(T entity)
        {
            T result = null;
            result = _objectSet.Add(entity);
            return result;
        }

        public void Editar(T entity)
        {
            entities.Entry(entity).State = EntityState.Modified;
            //_objectSet.Attach(entity);
        }

        public void Eliminar(T entity)
        {
            //borrado logico

            entities.Entry(entity).State = EntityState.Modified;

            // este codigo elimina el registro de la base de datos
            //if (entities.Entry(entity).State == EntityState.Detached)
            //{
            //    _objectSet.Attach(entity);
            //    dbSet.Remove(entityToDelete);
            //}
        }

        public void Eliminar(object id)
        {
            T entityToDelete = _objectSet.Find(id);
            Eliminar(entityToDelete);
        }

        public int Contar(Expression<Func<T, bool>> where)
        {
            if (where != null)
            {
                return _objectSet.Where(where).Count();
            }

            return _objectSet.AsEnumerable().Count();
        }
        public IQueryable<T> EncontrarPor(Expression<Func<T, bool>> predicate=null)
        {

            if (predicate != null)
            {
                return _objectSet.Where(predicate).AsQueryable();
            }

            return _objectSet.AsQueryable();

        }
        private OrderByClass<T> ObtenerOrderBy(ParametrosQuery<T> parametrosDeQuery)
        {
            if (parametrosDeQuery.OrderBy == null && parametrosDeQuery.OrderByDescending == null)
            {
                return null;
            }

            return (parametrosDeQuery.OrderBy != null)
                ? new OrderByClass<T>(parametrosDeQuery.OrderBy, true) :
                new OrderByClass<T>(parametrosDeQuery.OrderByDescending, false);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    entities.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: reemplace un finalizador solo si el anterior Dispose(bool disposing) tiene código para liberar los recursos no administrados.
        // ~RepositoryGenerico() {
        //   // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
        //   Dispose(false);
        // }

        // Este código se agrega para implementar correctamente el patrón descartable.
        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
            // TODO: quite la marca de comentario de la siguiente línea si el finalizador se ha reemplazado antes.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    internal class OrderByClass<T> where T : class
    {

        public OrderByClass()
        {

        }

        public OrderByClass(Func<T, object> orderBy, bool isAscending)
        {
            OrderBy = orderBy;
            IsAscending = isAscending;
        }


        public Func<T, object> OrderBy { get; set; }
        public bool IsAscending { get; set; }
    }

    
}
