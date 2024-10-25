using System.Text;
using Blogger.Contracts.Configs;
using Blogger.Contracts.Data;
using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Services;
using Blogger.Core;
using Blogger.Infrastructure.Data;
using Blogger.Infrastructure.Services;
using Blogger.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

#region Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


// Convert the relative path to absolute and normalize for the current OS
var dbPath = connectionString!
    .Replace("DataSource=", "", StringComparison.OrdinalIgnoreCase)
    .Split(';')[0]; // Get just the path part

// Convert to absolute path
if (!Path.IsPathRooted(dbPath))
{
    dbPath = Path.GetFullPath(
        Path.Combine(builder.Environment.ContentRootPath, dbPath));
}

// Ensure the directory exists
var dbDirectory = Path.GetDirectoryName(dbPath);
if (!Directory.Exists(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory!);
}

// Create new connection string with normalized path
var normalizedConnectionString = connectionString.Replace(
    connectionString.Split(';')[0], 
    $"DataSource={dbPath}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(normalizedConnectionString));

#endregion Database Configuration

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtTokenConfig:Key"])),
            ValidIssuer = builder.Configuration["JwtTokenConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtTokenConfig:Audience"],
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtTokenConfig>(builder.Configuration.GetSection("JwtTokenConfig"))
    .AddSingleton(x => x.GetRequiredService<IOptions<JwtTokenConfig>>().Value);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBlogsService, BlogsService>();
builder.Services.AddAutoMapper(automappper =>
{
    automappper.AddProfile<AutoMapperProfile>();
});

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.SetCommandTimeout(300); // For long running migrations
    if (db.Database.GetPendingMigrations().Any()) db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();