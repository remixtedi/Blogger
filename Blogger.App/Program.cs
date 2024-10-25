using Blogger.App.ApiServices;
using Blogger.App.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Blogger.App.Components;
using Blogger.App.Components.Account;
using Blogger.Contracts.Configs;
using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Services;
using Blogger.Infrastructure.Services;
using Blogger.Migrations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
// Add Identity services
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<JwtTokenConfig>(builder.Configuration.GetSection("JwtTokenConfig"))
    .AddSingleton(x => x.GetRequiredService<IOptions<JwtTokenConfig>>().Value);


builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Configure Identity Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    
    // Set authentication ticket validity to 60 minutes
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = false;
    
    // Configure paths
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    
    // Optional: Customize cookie name and other settings if needed
    options.Cookie.Name = "Blogger.Auth";
});

#region Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


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

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient<IBloggerApiService, BloggerApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]);
}).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

builder.Services.AddScoped<AuthenticatedHttpClientHandler>();
    
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.SetCommandTimeout(300); // For long running migrations
    if (db.Database.GetPendingMigrations().Any()) db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();