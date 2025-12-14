using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

namespace JobApp.Data;

public static class SeedJobData
{
    public static void EnsurePopulated(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<JobAppContext>();

        // Apply pending migrations 
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        // If no jobs exist, populate from the in-memory JobAppData sample list
        if (!context.JobDetails.Any())
        {
            var sample = JobAppData.GetJobListings();
            foreach (var s in sample)
            {
                context.JobDetails.Add(new Models.JobDetails
                {
                    Title = s.Title,
                    Company = s.Company,
                    Location = s.Location,
                    Description = s.Description,
                    Salary = s.Salary,
                    JobType = s.JobType,
                    PostedDate = s.PostedDate
                });
            }
            context.SaveChanges();
        }
    }
}
