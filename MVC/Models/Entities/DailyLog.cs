namespace MVC.Models.Entities;

public class DailyLog
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal? WeightKg { get; set; }
    // many DailyLogs -> 1 ApplicationUser
    public string ApplicationUserId { get; set; } = "";
    public ApplicationUser ApplicationUser { get; set; } = null!;
    // 1 DailyLog -> 0..1 DailyMacros
    public DailyMacros? DailyMacros { get; set; }
    // 1 DailyLog -> 0..1 DailyMovement
    public DailyMovement? DailyMovement { get; set; }
}