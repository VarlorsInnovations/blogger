namespace Backend.Interceptors;

public class VisitInterceptor(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Cookies.ContainsKey("visiter-id"))
        {
            var guid = Guid.NewGuid();
        }

        await _next(context);
    }
}