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

            List<Tag> tags =
            [
                new Tag() { Content = "Depressionen" },
                new Tag() { Content = "Happiness" },
                new Tag() { Content = "Medikamente" }
            ];
            
            await dbContext.Tags.AddRangeAsync(tags);

            List<Post> posts =
            [
                new()
                {
                    Tags = tags,
                    Title = "First post",
                    IsPublished = true,
                    Summary = "This is my very first post",
                    UrlIdentifier = "first-post",
                    CreatedAt = DateTime.UtcNow,
                    Parts = [new ContentPart() { Content = "Hello my friends", Type = ContentPartType.Paragraph }]
                },
                new()
                {
                    Tags = tags,
                    Title = "Second post",
                    IsPublished = true,
                    Summary = "This is my very second post",
                    UrlIdentifier = "second-post",
                    CreatedAt = DateTime.UtcNow,
                    Parts = [new ContentPart() { Content = "Nice to meeting you", Type = ContentPartType.Paragraph }]
                }
            ];
            
            await dbContext.Posts.AddRangeAsync(posts);
        }
        
        await _next(context);
    }
}