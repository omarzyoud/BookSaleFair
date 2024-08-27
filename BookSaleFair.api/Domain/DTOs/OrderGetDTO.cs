namespace BookSaleFair.api.Domain.DTOs
{
    public class OrderGetDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public string UserEmail { get; set; }
        public List<OrderItemGetDTO> OrderItems { get; set; }
    }
}
