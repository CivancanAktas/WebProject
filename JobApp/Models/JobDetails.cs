using System.ComponentModel.DataAnnotations;
namespace JobApp.Models;
public class JobDetails
{
    // Unique identifier for the job posting
public int Id { get; set; }

    [Required, MaxLength(100)]
    // Job title
    public string? Title { get; set; }

    [Required, MaxLength(200)]
    // Job description and responsibilities
    public string? Description { get; set; }

    [Required, MaxLength(100)]
    // Company that posted the job
    public string? Company { get; set; }

    [Required, MaxLength(200)]
    public string? Location { get; set; }

    [Required]
    public int? Salary { get; set; }

    [Required, MaxLength(50)]
    // Job type (Full-time, Part-time, Contract etc.)
    public string? JobType { get; set; }

    [Required]
    // Date the job was posted
    public DateTime PostedDate { get; set; }

    // relationships
    public Employer? Employer { get; set; }
    public List<Employee>? Employees { get; set; }

}