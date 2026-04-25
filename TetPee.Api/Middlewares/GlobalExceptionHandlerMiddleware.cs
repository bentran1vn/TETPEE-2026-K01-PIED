using TetPee.Service.DiscordService;
using TetPee.Service.Models;

namespace TetPee.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IService _service;

    public GlobalExceptionHandlerMiddleware(
        IHostEnvironment environment,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IService service)
    {
        _environment = environment;
        _logger = logger;
        _service = service;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request {Path}", context.Request.Path);

            if (context.Response.HasStarted)
            {
                // Lỗi nâng cao sẽ nói sau
                _logger.LogWarning("The response has already started, the global exception middleware will not write an error response");
                throw;
            }

            var statusCode = MapStatusCode(ex);

            await _service.SendExceptionAlertAsync(
                context,
                ex,
                statusCode,
                context.RequestAborted);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponseFactory.ErrorResponse(
                message: ResolveClientMessage(ex, statusCode),
                errors: _environment.IsDevelopment() ? new { detail = ex.Message } : null,
                traceId: context.TraceIdentifier);

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private static int MapStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string ResolveClientMessage(Exception ex, int statusCode)
    {
        return statusCode >= 500 ? "An unexpected error occurred" : ex.Message;
    }
}