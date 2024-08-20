using System.ComponentModel.DataAnnotations;
namespace BookSaleFair.api.Domain.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [StringLength(50)]
        public string Subject { get; set; } // e.g., "Science", "Literature"

        [Required]
        public int QuantityAvailable { get; set; }

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
