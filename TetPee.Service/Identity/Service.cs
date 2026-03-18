using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TetPee.Repository;
using TetPee.Service.JwtService;

namespace TetPee.Service.Identity;

public class Service : IService
{
    private readonly JwtService.IService _jwtService;
    private readonly AppDbContext _dbContext;
    private readonly JwtOptions _jwtOption = new();

    public Service(IConfiguration configuration, JwtService.IService jwtService, AppDbContext dbContext)
    {
        _jwtService = jwtService;
        _dbContext = dbContext;
        configuration.GetSection(nameof(JwtOptions)).Bind(_jwtOption);
    }

    public async Task<Response.IdentityResponse> Login(string email, string password)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        if(user.HashedPassword != password)
        {
            throw new Exception("Invalid password");
        }
        
        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Email", user.Email),
            new Claim("Role", user.Role),
            new Claim(ClaimTypes.Role, user.Role),
            // Phải có claim này để phân quyền cho các API endpoint, nếu thiếu claim này thì sẽ không phân quyền được
            new Claim(ClaimTypes.Expired, 
                DateTimeOffset.UtcNow.AddMinutes(_jwtOption.ExpireMinutes).ToString()),
        };
        
        var token = _jwtService.GenerateAccessToken(claims);
        
        var result = new Response.IdentityResponse()
        {
            AccessToken = token
        };

        return result;
    }
}