namespace MVC.Models.Entities;

public class CoachAssignment
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // many CoachAssignments -> 1 ApplicationUser
    public string MemberId { get; set; } = "";
    public string CoachId { get; set; } = "";
    public ApplicationUser Member { get; set; } = null!;
    public ApplicationUser Coach { get; set; } = null!;
}
