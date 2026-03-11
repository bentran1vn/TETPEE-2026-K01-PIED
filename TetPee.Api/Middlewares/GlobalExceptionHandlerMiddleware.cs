namespace TetPee.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        } catch (Exception ex)
        {
            Console.WriteLine("[GlobalExceptionHandlerMiddleware] Exception: " + ex);   
        }
    }
}