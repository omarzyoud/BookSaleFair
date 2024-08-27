namespace BookSaleFair.api.Domain.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public List<OrderItemReturnDTO> Items { get; set; }
    }
}
