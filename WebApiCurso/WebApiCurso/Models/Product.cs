using System.ComponentModel.DataAnnotations;

namespace WebApiCurso.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string? Description { get; set; }
        [Range(0.01, int.MaxValue, ErrorMessage = "Stock no puede ser negativo")]
        public int Stock { get; set; }

    }
}
