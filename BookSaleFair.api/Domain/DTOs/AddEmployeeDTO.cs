using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BookSaleFair.api.Domain.DTOs
{
    public class AddEmployeeDTO
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }

    }
}
