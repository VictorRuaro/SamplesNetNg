namespace Almacen.Infraestructura
{
    public class PagedBase
    {
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalRegistros { get; set; }

    }
}
