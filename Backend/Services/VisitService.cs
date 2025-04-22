using Backend.Data;
using Backend.Models;

namespace Backend.Services;

public class VisitService(ApplicationDbContext dbContext)
{
    public async Task AddSiteAsync(HttpContext httpContext, Post? post=null)
    {
        string visitId = Guid.NewGuid().ToString();
        if (httpContext.Request.Cookies.TryGetValue("visiter-id", out var visiterId))
        {
            visitId = visiterId;
        }
        
        await dbContext.Visits.AddAsync(new Visits() { VisiterId = visitId, Path=httpContext.Request.Path, Post = post });
        await dbContext.SaveChangesAsync();

        if (!httpContext.Request.Cookies.ContainsKey("visiter-id"))
        {
            httpContext.Response.Cookies.Append("visiter-id", visitId);
        }
    }
}