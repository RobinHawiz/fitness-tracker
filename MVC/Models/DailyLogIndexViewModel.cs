using System.ComponentModel;

namespace MVC.Models;

public class DailyLogIndexViewModel
{
    // DailyLog
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    [DisplayName("Weight (kg)")]
    public decimal? WeightKg { get; set; }

    // DailyMacros
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }
    public decimal? Protein { get; set; }
    public int? Calories { get; set; }

    // DailyMovement
    [DisplayName("Distance (km)")]
    public decimal? DistanceKm { get; set; }
    [DisplayName("Step count")]
    public int? StepCount { get; set; }
}
