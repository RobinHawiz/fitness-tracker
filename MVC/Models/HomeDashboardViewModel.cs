namespace MVC.Models;

public class HomeDashboardViewModel
{
    // Weekly data
    public decimal? AverageWeeklyWeight { get; set; } = null;
    public int? AverageWeeklyCalories { get; set; } = null;
    public decimal? TotalWeeklyDistance { get; set; } = null;
    public int? TotalWeeklySteps { get; set; } = null;

    // Monthly data
    public decimal? AverageMonthlyWeight { get; set; } = null;
    public int? AverageMonthlyCalories { get; set; } = null;
    public decimal? TotalMonthlyDistance { get; set; } = null;
    public int? TotalMonthlySteps { get; set; } = null;

    // Yearly data
    public decimal? AverageYearlyWeight { get; set; } = null;
    public int? AverageYearlyCalories { get; set; } = null;
    public decimal? TotalYearlyDistance { get; set; } = null;
    public int? TotalYearlySteps { get; set; } = null;
}
