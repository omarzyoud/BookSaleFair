namespace BookSaleFair.api.Domain.DTOs
{
    public class OrderItemReturnDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
