using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Models;

public class ClientDashboardViewModel
{
    public string UserName { get; set; } = default!;
    public decimal? AverageMonthlyWeight { get; set; } = null;
    public decimal? ChangeInMonthlyWeight { get; set; } = null;
    public int? AverageMonthlyCalories { get; set; } = null;
    public decimal? TotalMonthlyDistance { get; set; } = null;
    public int? TotalMonthlySteps { get; set; } = null;
    public string SelectedDate { get; set; } = "";
    public List<SelectListItem> AvailableDates { get; set; } = new();
}
