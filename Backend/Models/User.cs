using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public sealed class User : IdentityUser<int>
{
    
}

public sealed class Role : IdentityRole<int>
{
    
}