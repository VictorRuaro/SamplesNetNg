namespace Almacen.Infraestructura
{
    using System;
    using System.Linq.Expressions;
    public class ParametrosQuery<T>
    {
        public ParametrosQuery(int pagina, int rowsPagina)
        {
            PaginaActual = pagina;
            RegistrosPorPagina = rowsPagina;
            Where = null;
            OrderBy = null;
            OrderByDescending = null;
        }

        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public Expression<Func<T, bool>> Where { get; set; }
        public Func<T, object> OrderBy { get; set; }
        public Func<T, object> OrderByDescending { get; set; }
    }

    public class Parametros<T>
    {
        public Parametros()
        {
            Where = null;
            OrderBy = null;
            OrderByDescending = null;
        }
        public Expression<Func<T, bool>> Where { get; set; }
        public Func<T, object> OrderBy { get; set; }
        public Func<T, object> OrderByDescending { get; set; }
    }
}
