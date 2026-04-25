using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TetPee.Service.DiscordService;

public class Service : IService
{
    private readonly HttpClient _httpClient;
    private readonly DiscordAlertOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<Service> _logger;

    public Service(
        HttpClient httpClient,
        IOptions<DiscordAlertOptions> options,
        IHostEnvironment environment,
        ILogger<Service> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _environment = environment;
        _logger = logger;
        configuration.GetSection("DiscordAlertOptions").Bind(_options);
    }

    public async Task SendExceptionAlertAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.WebhookUrl))
        {
            return;
        }

        var content =
            $"{_options.Mention}\n" +
            "Unhandled exception in TetPee.Api\n" +
            $"Environment: {_environment.EnvironmentName}\n" +
            $"Request: {context.Request.Method} {context.Request.Path}\n" +
            $"StatusCode: {statusCode}\n" +
            $"TraceId: {context.TraceIdentifier}\n" +
            $"Exception: {exception.GetType().Name}\n" +
            $"Message: {exception.Message}";

        var payload = new { content };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(_options.WebhookUrl, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Discord alert failed with status code {StatusCode}. Response: {ResponseBody}",
                    response.StatusCode,
                    responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sending Discord alert failed");
        }
    }
}
