using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAz { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        List<Product> Products { get; set; }
    }
}
