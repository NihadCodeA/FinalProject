using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class ProductImages
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? Image { get; set; }
        public bool IsPoster { get; set; }
        public Product Product { get; set; }
    }
}
