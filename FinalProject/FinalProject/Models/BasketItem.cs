namespace FinalProject.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public string? Image { get; set; }
        public int Count { get; set; }
        public Product Product { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
