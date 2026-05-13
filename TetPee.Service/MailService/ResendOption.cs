using System.ComponentModel.DataAnnotations;

namespace TetPee.Service.MailService;

public class ResendOptions
{
    [Required]public string ApiToken { get; set; }
    [Required]public string From { get; set; }
}