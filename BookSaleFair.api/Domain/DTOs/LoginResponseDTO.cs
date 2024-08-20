namespace BookSaleFair.api.Domain.DTOs
{
    public class LoginResponseDTO
    {
        public string JwtToken { get; set; }
        public List<string> Roles { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        
    }
}
