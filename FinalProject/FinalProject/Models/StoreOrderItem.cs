namespace FinalProject.Models
{
    public class StoreOrderItem
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
