using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace main.Database.Storage;

public class MainUserIdentityDbContext : IdentityDbContext
{
    public MainUserIdentityDbContext(DbContextOptions<MainUserIdentityDbContext> options) : base(options)
    {
    }
}