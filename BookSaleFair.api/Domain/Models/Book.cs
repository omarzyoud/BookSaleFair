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
        public double Price { get; set; }
        public byte[] FileData { get; set; }
        [Required]
        [StringLength(50)]
        public string Subject { get; set; } 

        [Required]
        public int QuantityAvailable { get; set; }

        
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
