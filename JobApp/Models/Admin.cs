using System.ComponentModel.DataAnnotations;
namespace JobApp.Models;
public class Admin
{
    
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string? FirstName { get; set; }
    
    [Required, MaxLength(100)]
    public string? LastName { get; set; }

    [Required, MaxLength(100)]
    public string? ContactEmail { get; set; }

    


    
}
