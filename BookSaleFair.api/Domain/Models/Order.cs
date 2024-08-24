using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookSaleFair.api.Domain.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime? OrderDate { get; set; }

        [Required]
        public string Status { get; set; } 

        // Navigation properties
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
