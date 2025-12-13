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
    
    // Optional resume text or link to resume (not persisted yet)
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? Resume { get; set; }

    // Convenience property for display purposes
    public string FullName => $"{FirstName} {LastName}".Trim();
    
}