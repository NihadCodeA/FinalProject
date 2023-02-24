using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Product
    {
        public int Id { get; set; } 
        public int? StoreId { get; set; }
        [Required(ErrorMessage = "Text_Required"),StringLength(maximumLength:15)]
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Text_Required")]
        public int ProductCount { get; set; }
        public int? ProductView { get; set; }
        
        [Required(ErrorMessage = "Text_Required"),StringLength(maximumLength:100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Text_Required"),StringLength(maximumLength:1500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Text_Required"),StringLength(maximumLength:1500)]
        public string StorageTip { get; set; }
        [Required(ErrorMessage = "Text_Required"),StringLength(maximumLength:1500)]
        public string Disclaimer { get; set; }
        public DateTime? CreatedTime { get; set; }
        [Required(ErrorMessage = "Text_Required")]
        public double CostPrice { get; set; }
        [Required(ErrorMessage = "Text_Required")]
        public double SalePrice { get; set; }
        [Required(ErrorMessage = "Text_Required")]
        public double DiscountPercentage { get; set; }
        public bool IsAvaible { get; set; }
        [Required(ErrorMessage = "Text_Required")]
        public string Shipping { get; set; }
        [Required(ErrorMessage = "Text_Required")]
        public double Weight { get; set; }
        [Required(ErrorMessage = "Text_Required")]
        public double NetQuantity { get; set; }
        public string? Brand { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? Length { get; set; }
        public string? DimensionType { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public DateTime? DiscountStartingDate { get; set; }
        public DateTime? DiscountEndingDate { get; set; }

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
