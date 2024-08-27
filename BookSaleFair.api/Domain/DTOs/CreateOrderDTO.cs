using System.ComponentModel.DataAnnotations;

namespace BookSaleFair.api.Domain.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}
