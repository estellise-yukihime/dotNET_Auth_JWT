using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace main.Database.Storage.MainUser.Context;

public class MainUserDbContext : IdentityDbContext
{
    public MainUserDbContext(DbContextOptions<MainUserDbContext> options) : base(options)
    {
    }
}