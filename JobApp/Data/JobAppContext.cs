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

        public DbSet<JobApp.Models.Employee> Employees => Set<Employee>();

        public DbSet<JobApp.Models.Employer> Employers => Set<Employer>();

       public DbSet<JobApp.Models.JobDetails> JobDetails => Set<JobDetails>();
    }       

}