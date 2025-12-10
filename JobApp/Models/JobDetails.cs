using System.ComponentModel.DataAnnotations;
namespace JobApp.Models;
public class JobDetails
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string? Title { get; set; }

    [Required, MaxLength(200)]
    public string? Description { get; set; }

    [Required, MaxLength(100)]
    public string? Company { get; set; }

    [Required, MaxLength(200)]
    public string? Location { get; set; }

    [Required]
    public int? Salary { get; set; }

    [Required, MaxLength(50)]
    public string? JobType { get; set; }

    [Required]
    public DateTime PostedDate { get; set; }

    // relationships
    public Employer? Employer { get; set; }
    public List<Employee>? Employees { get; set; }

}