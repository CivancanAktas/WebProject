using System.ComponentModel.DataAnnotations;
namespace JobApp.Models;
public class JobDetails
{
     [Required , MaxLength(100)]
    public string ?Title { get; set; }
   
    [Required , MaxLength(200)]
    public string ?Description { get; set; }
    
    [Required , MaxLength(100)]
    public string ?Company { get; set; }
    
    [Required , MaxLength(200)]
    public string ?Location { get; set; }
    
    [Required]
    public DateTime PostedDate { get; set; }

    public int? salary { get; set; }
    
    //relationships
    public Employer ?Employer { get; set; }
    public List<Employee> ?Employees { get; set; }

    


}