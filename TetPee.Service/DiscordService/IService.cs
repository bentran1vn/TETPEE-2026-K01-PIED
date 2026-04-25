using Microsoft.AspNetCore.Http;

namespace TetPee.Service.DiscordService;

public interface IService
{
    Task SendExceptionAlertAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        CancellationToken cancellationToken = default);
}
