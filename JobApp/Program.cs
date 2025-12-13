// Program.cs: configures services, DB context, Identity, middleware and routing for the JobApp.
// High level: (1) register EF Core context and Identity, (2) configure middleware (auth), (3) map controller routes
using JobApp.Models;
using JobApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Create the WebApplication builder which collects services and middleware configuration
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register application DbContext (Identity + application tables) using SQLite as data store
builder.Services.AddDbContext<JobAppIdentityContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=jobapp.db"));
builder.Services.AddDbContext<JobAppContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=jobapp.db"));

// Configure Identity: password policy and registering EF store for Identity implementation
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<JobAppIdentityContext>()
.AddDefaultTokenProviders();

// Configure application cookie (Login path ayarı)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/LoginPage/Login";
    options.AccessDeniedPath = "/LoginPage/Access";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configure middleware pipeline
app.UseHttpsRedirection();
app.UseRouting();

// Authentication must be enabled prior to Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map static web assets and controller routes
app.MapStaticAssets();

// Sadece bir tane default route yeterli
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Apply migrations and seed roles/data on startup (migrations used for repeatable updates)
using (var scope = app.Services.CreateScope())
{
    // Identity DB
    var idContext = scope.ServiceProvider.GetRequiredService<JobAppIdentityContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Use migrations so DB schema evolves predictably (better for dev & staging; gate auto-migrate in prod)
    idContext.Database.Migrate();

    // Seed roles
    string[] roles = new[] { "Admin", "Employer", "Employee" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Application DB: run migrations and seed job data using SeedJobData helper
    var jobDb = scope.ServiceProvider.GetRequiredService<JobAppContext>();
    jobDb.Database.Migrate();
}

// Run seeding logic that depends on IApplicationBuilder (mirrors CineClub pattern)
SeedJobData.EnsurePopulated(app);

app.Run();