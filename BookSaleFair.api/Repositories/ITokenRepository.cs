using Microsoft.AspNetCore.Identity;

namespace BookSaleFair.api.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
