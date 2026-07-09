using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class DailyLogFormViewModel
{
    // DailyLog
    public DateOnly Date { get; set; }
    [Range(0, 999.99, ErrorMessage = "Weight cannot be negative.")]
    [DisplayName("Weight (kg)")]
    public decimal? WeightKg { get; set; }

    // DailyMacros
    [Range(0, 99999.99, ErrorMessage = "Fat cannot be negative.")]
    [DisplayName("Fat (g)")]
    public decimal? Fat { get; set; }
    [Range(0, 99999.99, ErrorMessage = "Carbs cannot be negative.")]
    [DisplayName("Carbs (g)")]
    public decimal? Carbs { get; set; }
    [Range(0, 99999.99, ErrorMessage = "Protein cannot be negative.")]
    [DisplayName("Protein (g)")]
    public decimal? Protein { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Calories cannot be negative.")]
    [DisplayName("Calories (kcal)")]
    public int? Calories { get; set; }

    // DailyMovement
    [Range(0, 9999.99, ErrorMessage = "Distance cannot be negative.")]
    [DisplayName("Distance (km)")]
    public decimal? DistanceKm { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Step count cannot be negative.")]
    [DisplayName("Step count")]
    public int? StepCount { get; set; }
}
