using System.ComponentModel.DataAnnotations;
namespace JobApp.Models;

public class Employee
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string? FirstName { get; set; }

    [Required, MaxLength(100)]
    public string? LastName { get; set; }

    [Required, MaxLength(100)]
    public string? Email { get; set; }

    [Required, MaxLength(15)]
    public string? PhoneNumber { get; set; }

    public List<JobDetails>? AppliedJobs { get; set; }
    
}