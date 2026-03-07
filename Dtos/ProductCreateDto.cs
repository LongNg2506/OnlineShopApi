namespace OnlineShopApi.Dtos
{
    public class ProductCreateDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; } = 0; // % 0..100
        public int Stock { get; set; } = 0;
    }
}
