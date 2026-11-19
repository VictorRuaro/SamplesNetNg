namespace Almacen.Infraestructura
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Data.Entity.Validation;
    public class GenericUnitOfWork : IGenericUnitOfWork
    {
        //private AlmacenDopajeEntities entities = null;

        private readonly DbContext entities = null;
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public GenericUnitOfWork(DbContext entities)
        {
            this.entities = entities;
        }
        //public GenericUnitOfWork()
        //{
        //    entities = new AlmacenDopajeEntities();
        //}

        public IRepository<T> Repository<T>() where T : class
        {

            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> repo = new RepositoryGenerico<T>(entities);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public ResultSave SaveChanges()
        {
            ResultSave result = new ResultSave();
            try
            {
                result.Afectados = entities.SaveChanges();
                result.Mensaje = "Se guardo correctamente";
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    string errorEntity = string.Format("Entity of type \"{0}\" in state \"{1}\" validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string errorProperty = string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);

                        errorEntity += errorProperty;
                    }
                    result.Mensaje += errorEntity;
                }

            }
            catch (Exception ex)
            {
                result.Mensaje = ex.Message;

                if (ex.InnerException != null)
                {
                    result.Mensaje += "-" + ex.InnerException.Message.Replace(ex.Message, "");

                    if (ex.InnerException.InnerException != null)
                    {
                        result.Mensaje += "-" + ex.InnerException.InnerException.Message;
                    }
                }
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
                    entities.Dispose();
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
