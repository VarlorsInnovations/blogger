using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Interceptors;

/// <summary>
/// For testing purpose only. Needs to be removed in production
/// </summary>
public class CreateUserInterceptor
{
    private readonly RequestDelegate _next;

    public CreateUserInterceptor(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context, 
        ApplicationDbContext dbContext,
        UserManager<User> userManager)
    {
        if (!dbContext.Users.Any())
        {
            User user = new User()
            {
                EmailConfirmed = true,
                Email = "valacor@local.com",
                UserName = "valacor",
                LockoutEnabled = false,
            };
            
            var result = await userManager.CreateAsync(user, "Password#123");
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
        
        await _next(context);
    }
}