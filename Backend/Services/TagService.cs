using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

// todo: dead-code; maybe interesting in a refactoring
public sealed class TagService
{
    private readonly ApplicationDbContext _dbContext;
    
    public TagService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Tag tag)
    {
        if (tag is null)
        {
            throw new ArgumentNullException(nameof(tag));
        }
        
        _dbContext.Tags.Add(tag);
    }

    public async Task DeleteAsync(int id) 
    {
        var tag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);

        if (tag is null)
        {
            throw new Exception("Tag does not exists!");
        }
        
        _dbContext.Tags.Remove(tag);
    }

    public async Task ApplyChangesAsync() => await _dbContext.SaveChangesAsync();
}