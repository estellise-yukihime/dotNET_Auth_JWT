using main.Database.Model.Identity;
using main.Database.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("SQLServerConnection");

builder.Services.AddDbContext<MainUserIdentityDbContext>(x => x.UseSqlServer(connection));
// add more database context

builder.Services.AddIdentityCore<MainUser>(x =>
    {
        x.SignIn.RequireConfirmedEmail = false;
        x.SignIn.RequireConfirmedAccount = false;
        x.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MainUserIdentityDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "",
            ValidAudience = "",
            IssuerSigningKey = null,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
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
    
    x.AddPolicy("HasClaimEmailAddress", p =>
    {
        p.RequireClaim("EmailAddress");
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