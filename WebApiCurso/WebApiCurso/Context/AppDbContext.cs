using Microsoft.EntityFrameworkCore;
using WebApiCurso.Models;

namespace WebApiCurso.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
                
        }
        public DbSet<Product> Products { get; set; } 
    }
}
