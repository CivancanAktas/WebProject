using System;
using System.Collections.Generic;

namespace JobApp.Data
{
    public class JobListing
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Company { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public int Salary { get; set; }
        public string? JobType { get; set; }
        public DateTime PostedDate { get; set; }
    }

    public class JobAppData
    {
        public static List<JobListing> GetJobListings()
        {
            return new List<JobListing>
            {
                new JobListing
                {
                    Id = 1,
                    Title = "Senior C# Developer",
                    Company = "TechCorp Solutions",
                    Location = "Istanbul, Turkey",
                    Description = "We are looking for an experienced C# developer to join our team.",
                    Salary = 85000,
                    JobType = "Full-Time",
                    PostedDate = DateTime.Now.AddDays(-5)
                },
                new JobListing
                {
                    Id = 2,
                    Title = "Frontend Developer",
                    Company = "Digital Innovations",
                    Location = "Ankara, Turkey",
                    Description = "Seeking a talented frontend developer with React expertise.",
                    Salary = 65000,
                    JobType = "Full-Time",
                    PostedDate = DateTime.Now.AddDays(-10)
                },
                new JobListing
                {
                    Id = 3,
                    Title = "Data Analyst",
                    Company = "Analytics Pro",
                    Location = "Remote",
                    Description = "Analyze data and provide insights for business decisions.",
                    Salary = 55000,
                    JobType = "Contract",
                    PostedDate = DateTime.Now.AddDays(-2)
                },
                new JobListing
                {
                    Id = 4,
                    Title = "DevOps Engineer",
                    Company = "CloudServices Inc",
                    Location = "Izmir, Turkey",
                    Description = "Manage cloud infrastructure and deployment pipelines.",
                    Salary = 75000,
                    JobType = "Full-Time",
                    PostedDate = DateTime.Now.AddDays(-7)
                },
                new JobListing
                {
                    Id = 5,
                    Title = "Junior Java Developer",
                    Company = "WebDynamics",
                    Location = "Bursa, Turkey",
                    Description = "Entry-level Java position for recent graduates.",
                    Salary = 45000,
                    JobType = "Full-Time",
                    PostedDate = DateTime.Now
                }
            };
        }
    }
}