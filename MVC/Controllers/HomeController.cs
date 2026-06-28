using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Entities;

namespace MVC.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public HomeController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }
    public async Task<IActionResult> Index()
    {
        var isSignedIn = _signInManager.IsSignedIn(User);

        if (isSignedIn)
        {
            return RedirectToAction("Dashboard");
        }

        var model = new HomeIndexViewModel
        {
            UserNames = await _context.Users.Select(t => t.UserName!).ToListAsync(),
        };

        return View(model);
    }

    [Authorize]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var userId = _signInManager.UserManager.GetUserId(User);

        var dailyLogs = await _context.DailyLogs
                .Where(d => d.ApplicationUserId == userId!)
                .OrderByDescending(d => d.Date)
                .Select(d => new
                {
                    d.Id,
                    d.Date,
                    d.WeightKg,

                    d.DailyMacros.Fat,
                    d.DailyMacros.Carbs,
                    d.DailyMacros.Protein,
                    d.DailyMacros.Calories,

                    d.DailyMovement.DistanceKm,
                    d.DailyMovement.StepCount
                })
                .ToListAsync();

        var dateNow = DateOnly.FromDateTime(DateTime.Now);
        var model = new HomeDashboardViewModel();

        // Weekly data

        var firstDayOfWeek = dateNow.FirstDayOfWeek();
        var lastDayOfWeek = dateNow.LastDayOfWeek();

        var weeklyData = dailyLogs.Where(d => d.Date >= firstDayOfWeek && d.Date <= lastDayOfWeek).ToList();

        var weeklyWeights = weeklyData.Where(d => d.WeightKg.HasValue).Select(d => d.WeightKg!.Value).ToList();
        model.AverageWeeklyWeight = weeklyWeights.Count > 0 ? Math.Round(weeklyWeights.Average(), 2) : null;

        var weeklyCalories = weeklyData.Where(d => d.Calories.HasValue).Select(d => d.Calories!.Value).ToList();
        model.AverageWeeklyCalories = weeklyCalories.Count > 0 ? Convert.ToInt32(weeklyCalories.Average()) : null;

        var weeklyDistance = weeklyData.Where(d => d.DistanceKm.HasValue).Select(d => d.DistanceKm!.Value).ToList();
        model.TotalWeeklyDistance = weeklyDistance.Count > 0 ? Math.Round(weeklyDistance.Sum(), 2) : null;

        var weeklySteps = weeklyData.Where(d => d.StepCount.HasValue).Select(d => d.StepCount!.Value).ToList();
        model.TotalWeeklySteps = weeklySteps.Count > 0 ? weeklySteps.Sum() : null;

        // Monthly data

        var firstDayOfMonth = dateNow.FirstDayOfMonth();
        var lastDayOfMonth = dateNow.LastDayOfMonth();

        var monthlyData = dailyLogs.Where(d => d.Date >= firstDayOfMonth && d.Date <= lastDayOfMonth).ToList();

        var monthlyWeights = monthlyData.Where(d => d.WeightKg.HasValue).Select(d => d.WeightKg!.Value).ToList();
        model.AverageMonthlyWeight = monthlyWeights.Count > 0 ? Math.Round(monthlyWeights.Average(), 2) : null;

        var monthlyCalories = monthlyData.Where(d => d.Calories.HasValue).Select(d => d.Calories!.Value).ToList();
        model.AverageMonthlyCalories = monthlyCalories.Count > 0 ? Convert.ToInt32(monthlyCalories.Average()) : null;

        var monthlyDistance = monthlyData.Where(d => d.DistanceKm.HasValue).Select(d => d.DistanceKm!.Value).ToList();
        model.TotalMonthlyDistance = monthlyDistance.Count > 0 ? Math.Round(monthlyDistance.Sum(), 2) : null;

        var monthlySteps = monthlyData.Where(d => d.StepCount.HasValue).Select(d => d.StepCount!.Value).ToList();
        model.TotalMonthlySteps = monthlySteps.Count > 0 ? monthlySteps.Sum() : null;

        // Yearly data

        var firstDayOfYear = dateNow.FirstDayOfYear();
        var lastDayOfYear = dateNow.LastDayOfYear();

        var yearlyData = dailyLogs.Where(d => d.Date >= firstDayOfYear && d.Date <= lastDayOfYear).ToList();

        var yearlyWeights = yearlyData.Where(d => d.WeightKg.HasValue).Select(d => d.WeightKg!.Value).ToList();
        model.AverageYearlyWeight = yearlyWeights.Count > 0 ? Math.Round(yearlyWeights.Average(), 2) : null;

        var yearlyCalories = yearlyData.Where(d => d.Calories.HasValue).Select(d => d.Calories!.Value).ToList();
        model.AverageYearlyCalories = yearlyCalories.Count > 0 ? Convert.ToInt32(yearlyCalories.Average()) : null;

        var yearlyDistance = yearlyData.Where(d => d.DistanceKm.HasValue).Select(d => d.DistanceKm!.Value).ToList();
        model.TotalYearlyDistance = yearlyDistance.Count > 0 ? Math.Round(yearlyDistance.Sum(), 2) : null;

        var yearlySteps = yearlyData.Where(d => d.StepCount.HasValue).Select(d => d.StepCount!.Value).ToList();
        model.TotalYearlySteps = yearlySteps.Count > 0 ? yearlySteps.Sum() : null;

        return View(model);
    }

}

public static partial class DateTimeExtensions
{
    public static DateOnly FirstDayOfWeek(this DateOnly dt)
    {
        // Assume Monday is the first day of the week.
        // Sunday enum value is 0, so we set it to 6 to get the correct difference.
        var diff = dt.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)dt.DayOfWeek - (int)DayOfWeek.Monday;

        return dt.AddDays(-diff);
    }

    public static DateOnly LastDayOfWeek(this DateOnly dt) =>
        dt.FirstDayOfWeek().AddDays(6);

    public static DateOnly FirstDayOfMonth(this DateOnly dt) =>
        new(dt.Year, dt.Month, 1);

    public static DateOnly LastDayOfMonth(this DateOnly dt) =>
        dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

    public static DateOnly FirstDayOfYear(this DateOnly dt) =>
        new(dt.Year, 1, 1);

    public static DateOnly LastDayOfYear(this DateOnly dt) =>
        new(dt.Year, 12, 31);
}
