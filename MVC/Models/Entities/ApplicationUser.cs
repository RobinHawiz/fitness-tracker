using Microsoft.AspNetCore.Identity;

namespace MVC.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    // 1 ApplicationUser -> many DailyLogs
    public ICollection<DailyLog> DailyLogs { get; set; } = new List<DailyLog>();
    // 1 ApplicationUser -> many CoachAssignments
    public ICollection<CoachAssignment> CoachAssignmentsAsMember { get; set; } = new List<CoachAssignment>();
    public ICollection<CoachAssignment> CoachAssignmentsAsCoach { get; set; } = new List<CoachAssignment>();
}
