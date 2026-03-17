using System.ComponentModel.DataAnnotations;

namespace TetPee.Service.JwtService;

public class JwtOptions
{
    [Required]public string Issuer { get; set; }
    [Required]public string Audience { get; set; }
    [Required]public string SecretKey { get; set; }
    [Required]public int ExpireMinutes { get; set; }
}