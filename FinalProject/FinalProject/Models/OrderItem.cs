namespace FinalProject.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId1 { get; set; }
        public int StoreId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public double SalePrice { get; set; }
        public double CostPrice { get; set; }
        public double DiscountPercentage { get; set; }
        public int Count { get; set; }
        public Product? Product { get; set; }
        public Store Store { get; set; }
        public Order Order { get; set; }
    }
}
