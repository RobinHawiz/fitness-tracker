namespace MVC.Models;

public class CoachIndexViewModel
{
    public CoachAssignInputModel AssignCoach { get; set; } = new();
    public List<AssignedCoachViewModel> AssignedCoaches { get; set; } = new();
}
