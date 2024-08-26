using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookSaleFair.api.Domain.DTOs
{
    public class AddBookDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Auther { get; set; }
        [Required]
        public double price { get; set; }
        [Required]

        public IFormFile Cover { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
