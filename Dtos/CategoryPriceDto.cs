namespace OnlineShopApi.Dtos
{
    public class CategoryPriceDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalPrice { get; set; }
    }
}
