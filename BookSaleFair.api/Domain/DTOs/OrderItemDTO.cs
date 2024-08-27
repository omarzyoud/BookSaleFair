using System.ComponentModel.DataAnnotations;

namespace BookSaleFair.api.Domain.DTOs
{
    public class OrderItemDTO
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
