using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JobApp.Models;

namespace JobApp.Data;

public class JobAppIdentityContext : IdentityDbContext<IdentityUser>
{
    public JobAppIdentityContext(DbContextOptions<JobAppIdentityContext> options) : base(options)
    {
    }

    // Application entities can live here so Identity and app data share the same database/context
    public DbSet<JobDetails>? JobDetails { get; set; }
    public DbSet<Employee>? Employees { get; set; }
    public DbSet<Employer>? Employers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

      
    }
}
