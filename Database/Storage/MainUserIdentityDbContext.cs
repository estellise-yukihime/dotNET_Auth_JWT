using main.Database.Model.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace main.Database.Storage;

public class MainUserIdentityDbContext : IdentityDbContext
{
    public DbSet<MainUser> MainUsers
    {
        get;
        set;
    } = null!;

    public MainUserIdentityDbContext(DbContextOptions<MainUserIdentityDbContext> options) : base(options)
    {
    }
}