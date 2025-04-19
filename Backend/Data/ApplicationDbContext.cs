using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public sealed class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<Post> Posts { get; set; }
    
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<ContentPart> ContentParts { get; set; }
    
    public DbSet<PostRelation> PostRelations { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<PostRelation>()
            .HasOne(p => p.RelatedPost)
            .WithMany(x => x.Relations)
            .HasForeignKey(x => x.RelatedId);
    }
}