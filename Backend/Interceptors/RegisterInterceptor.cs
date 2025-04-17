namespace Backend.Interceptors;

public class RegisterInterceptor
{
    private readonly RequestDelegate _next;

    public RegisterInterceptor(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/Identity/Account/Register")
        {
            context.Response.Redirect("/Identity/Account/Login");
        }
        
        await _next(context);
    }
}