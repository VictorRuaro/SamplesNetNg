using System;

namespace Almacen.Infraestructura
{
    public interface IGenericUnitOfWork: IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        ResultSave SaveChanges();
    }


}
