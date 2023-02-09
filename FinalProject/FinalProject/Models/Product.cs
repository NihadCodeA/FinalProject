using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Product
    {
        public int Id { get; set; } 
        public int? StoreId { get; set; }
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        [StringLength(maximumLength:1500)]
        public string Description { get; set; }
        [StringLength(maximumLength:1500)]
        public string StorageTip { get; set; }
        [StringLength(maximumLength:1500)]
        public string Disclaimer { get; set; }
        public DateTime? CreatedTime { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public double DiscountPercentage { get; set; }
        public bool IsAvaible { get; set; }
        public string Type { get; set; }
        public string Shipping { get; set; }
        public double Weight { get; set; }
        public double NetQuantity { get; set; }
        public string IngredientType { get; set; }
        public string? Brand { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? Length { get; set; }
        public string? DimensionType { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }

        public Store? Store { get; set; }

        [NotMapped]
        public IFormFile? PosterImgFile { get; set; }
        public List<ProductImages>? ProductImages { get; set; }
        [NotMapped]
        public List<int>? ProductImageIds { get; set; }
        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
