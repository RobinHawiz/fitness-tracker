using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class CoachAssignInputModel
{
    [Required]
    [StringLength(50, ErrorMessage = "{0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
    [Display(Name = "Username")]
    public string UserName { get; set; } = default!;
}
