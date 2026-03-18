using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TetPee.Service.JwtService;

namespace TetPee.Api.Extensions;

public static class JwtExtensions
{
    public const string AdminPolicy = "AdminPolicy";
    public const string SellerPolicy = "SellerPolicy";
    public const string UserPolicy = "UserPolicy";
    public const string SellerOrAdminPolicy = "SellerOrAdminPolicy";

    public static void AddJwtServices(this IServiceCollection services, IConfiguration configuration)
    {
        JwtOptions jwtOption = new JwtOptions();
        configuration.GetSection(nameof(JwtOptions)).Bind(jwtOption);
        var key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true, 
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOption.Issuer,
                    ValidAudience = jwtOption.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AdminPolicy, policy =>
                policy.RequireRole("Admin"));
            // [Authorize(Policy = JwtExtensions.AdminPolicy)]
        
            options.AddPolicy(SellerPolicy, policy =>
                policy.RequireRole("Seller"));
            // [Authorize(Policy = JwtExtensions.SellerPolicy)]
        
            options.AddPolicy(UserPolicy, policy =>
                policy.RequireRole("User"));
        
            options.AddPolicy(SellerOrAdminPolicy, policy =>
                policy.RequireRole("Seller", "Admin"));
        
            // [Authorize(Policy = JwtExtensions.SellerOrAdminPolicy)]
        });
    }
}