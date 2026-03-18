using System.Security.Claims;

namespace TetPee.Service.JwtService;

public interface IService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims);
    
    ClaimsPrincipal ValidateToken(string token); 

}