using System.Security.Claims;

namespace TetPee.Service.JwtService;

public interface IJwtServices
{
    public string GenerateAccessToken(IEnumerable<Claim> claims);
    
    ClaimsPrincipal ValidateToken(string token); 

}