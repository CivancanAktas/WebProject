using System.ComponentModel.DataAnnotations;
namespace JobApp.ViewModels;
public class RegisterEmployerViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string? UserName { get; set; }

     [Required, MaxLength(100)]
    public string? FirstName { get; set; }
     
     [Required, MaxLength(100)]
    public string ?LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Contact Email")]
    public string? ContactEmail { get; set; }

     [Required]
    [EmailAddress]
    [Display(Name = "Company Name")]
    public string? CompanyName { get; set; }


    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string ?Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }
}
