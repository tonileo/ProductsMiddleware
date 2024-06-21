using Microsoft.AspNetCore.Identity;

namespace ProductsMiddleware.Repositories
{
    public interface ITokenRepostory
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
