namespace MVC.Models.Entities;

public class DailyMacros
{
    public int Id { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }
    public decimal? Protein { get; set; }
    public int? Calories { get; set; }
    // 1 DailyMacros -> 1 DailyLog
    public int DailyLogId { get; set; }
    public DailyLog DailyLog { get; set; } = null!;
}
