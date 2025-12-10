using System.ComponentModel.DataAnnotations;
namespace JobApp.Models;
public class Employer
{
    
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string? FirstName { get; set; }
    
    [Required, MaxLength(100)]
    public string? LastName { get; set; }

    [Required, MaxLength(200)]
    public string? Address { get; set; }

    [Required, MaxLength(100)]
    public string? ContactEmail { get; set; }

    [Required, MaxLength(100)]
    public string? CompanyName { get; set; }
    
    public List<JobDetails> ?PostedJobs { get; set; }
}