using System.ComponentModel.DataAnnotations;

public class JobDetails
{
     [Required , MaxLength(100)]
    public string Title { get; set; }
   
    [Required , MaxLength(200)]
    public string Description { get; set; }
    
    [Required , MaxLength(100)]
    public string Company { get; set; }
    
    [Required , MaxLength(200)]
    public string Location { get; set; }[Required , MaxLength(100)]
    
    public DateTime PostedDate { get; set; }

    public int? salary { get; set; }
    public Employer Employer { get; set; }

    public List<Employee> Employees { get; set; }

    


}