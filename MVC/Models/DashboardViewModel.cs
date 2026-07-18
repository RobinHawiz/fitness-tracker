using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Models;

public class DashboardViewModel
{
    // Monthly statistics
    public decimal? AverageMonthlyWeight { get; set; } = null;
    public decimal? ChangeInMonthlyWeight { get; set; } = null;
    public int? AverageMonthlyCalories { get; set; } = null;
    public decimal? TotalMonthlyDistance { get; set; } = null;
    public int? TotalMonthlySteps { get; set; } = null;
    // Select text and values
    public string SelectedDate { get; set; } = "";
    public List<SelectListItem> AvailableDates { get; set; } = new();
    // Chart data
    public List<string> ChartLabels { get; set; } = new();
    public List<decimal> AverageWeights { get; set; } = new();
}
