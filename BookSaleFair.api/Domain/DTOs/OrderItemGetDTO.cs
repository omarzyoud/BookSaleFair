namespace BookSaleFair.api.Domain.DTOs
{
    public class OrderItemGetDTO
    {
        public int OrderItemId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
