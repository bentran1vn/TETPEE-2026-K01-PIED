using Microsoft.Extensions.Configuration;
using Resend;

namespace TetPee.Service.MailService;

public class ResendService : IService
{
    private readonly ResendOptions _resendOptions = new();
    private readonly IResend _resend;
    
    public ResendService(IConfiguration configuration, IResend resend)
    {
        _resend = resend;
        configuration.GetSection(nameof(ResendOptions)).Bind(_resendOptions);
    }
    
    public async Task SendMail(MailContent mailContent)
    {
        var message = new EmailMessage
        {
            From = _resendOptions.From,
            To = new[] { mailContent.To },
            Subject = mailContent.Subject,
            HtmlBody = mailContent.Body
        };
        
        await _resend.EmailSendAsync(message);
    }
}