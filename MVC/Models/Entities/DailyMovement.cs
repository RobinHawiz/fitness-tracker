namespace MVC.Models.Entities;

public class DailyMovement
{
    public int Id { get; set; }
    public decimal? DistanceKm { get; set; }
    public int? StepCount { get; set; }
    // 1 DailyMovement -> 1 DailyLog
    public int DailyLogId { get; set; }
    public DailyLog DailyLog { get; set; } = null!;
}
