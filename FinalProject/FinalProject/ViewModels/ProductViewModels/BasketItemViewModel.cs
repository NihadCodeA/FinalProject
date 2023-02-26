namespace FinalProject.ViewModels.ProductViewModels
{
    public class BasketItemViewModel
    {
        public int ProductId { get; set; }
        public int Count { get; set; }

        public double Price { get; set; }
        public double Discount { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Image { get; set; }
    }
}
