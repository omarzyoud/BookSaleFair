using System.ComponentModel.DataAnnotations;

namespace BookSaleFair.api.Domain.DTOs
{
    public class RegisterRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string password { get; set; }
        //public DateTime createdat { get; set; }        
        //public string[] Roles { get; set; }
        //public int id { get; set; }
    }
}
