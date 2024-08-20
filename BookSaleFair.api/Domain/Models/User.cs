using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BookSaleFair.api.Domain.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [Required]
        public string Type { get; set; } // "Customer", "Employee", or "Admin"

        public DateTime? CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }

}
