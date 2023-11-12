using System.Security.Claims;
using System.Text;
using main.Database.Storage;
using main.Database.Storage.MainUser.Context;
using main.Database.Storage.MainUser.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("SQLServerConnection");

builder.Services.AddIdentity<MainUser, IdentityRole>()
    .AddEntityFrameworkStores<MainUserDbContext>();
// add more database context

builder.Services.AddIdentityCore<MainUser>(x =>
    {
        x.SignIn.RequireConfirmedEmail = false;
        x.SignIn.RequireConfirmedAccount = false;
        x.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MainUserDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        var jwtSection = builder.Configuration.GetSection("JWT");
        var jwtKey = jwtSection["Key"];
        var jwtIssuer = jwtSection["Issuer"];
        var jwtAudience = jwtSection["Audience"];
        
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        x.Events = new JwtBearerEvents
        {
            // To check what credentials the user has
            // claims
            // roles
            OnTokenValidated = _ => Task.CompletedTask
        };
    });

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("RequireAdmin", p =>
    {
        p.RequireRole("Admin");
    });
    
    x.AddPolicy("RequireUser", p =>
    {
        p.RequireRole("User");
    });
    
    x.AddPolicy("RequireAdminOrUser", p =>
    {
        p.RequireRole("Admin", "User");
    });
    
    x.AddPolicy("HasClaimEmailAddress", p =>
    {
        p.RequireClaim(ClaimTypes.Email);
    });
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.Run();