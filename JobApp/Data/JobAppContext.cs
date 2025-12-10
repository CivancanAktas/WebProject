using Microsoft.EntityFrameworkCore;
using JobApp.Models;
namespace JobApp.Data
{
    public class JobAppContext : DbContext
    {
        public JobAppContext (DbContextOptions<JobAppContext> options)
            : base(options)
        {
        }

        public DbSet<JobApp.Models.Employee> Employees { get; set; } = default!;

        public DbSet<JobApp.Models.Employer> Employers { get; set; } = default!;

       public DbSet<JobApp.Models.JobDetails> JobDetails { get; set; } = default!;
    }       

}