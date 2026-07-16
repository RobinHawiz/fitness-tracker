using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Extensions;
using MVC.Models;
using MVC.Models.Entities;
using System.Globalization;

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
    public async Task<IActionResult> Dashboard(string? selectedDate)
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

        var model = new HomeDashboardViewModel();
        var english = CultureInfo.GetCultureInfo("en-US");
        model.AvailableDates = dailyLogs
                                .Select(e => new
                                {
                                    e.Date.Year,
                                    e.Date.Month
                                })
                                .Distinct()
                                .OrderByDescending(e => e.Year)
                                .ThenByDescending(e => e.Month)
                                .Select(e => new SelectListItem
                                {
                                    Value = $"{e.Year}-{e.Month:D2}",
                                    Text = $"{english.DateTimeFormat.GetMonthName(e.Month)} {e.Year}"
                                })
                                .ToList();

        if(model.AvailableDates.Count == 0)
        {
            return View(model);
        }

        DateOnly selectedDateOnly;
        var latestAvailableDate = model.AvailableDates.First().Value;

        if (selectedDate != null && model.AvailableDates.Any(sli => sli.Value == selectedDate))
        {
            selectedDateOnly = DateOnly.Parse(selectedDate);
        }
        else
        {
            selectedDate = latestAvailableDate;
            selectedDateOnly = DateOnly.Parse(selectedDate);
        }

        model.SelectedDate = selectedDate;

        // Monthly statistics
        var firstDayOfMonth = selectedDateOnly.FirstDayOfMonth();
        var lastDayOfMonth = selectedDateOnly.LastDayOfMonth();

        var monthlyData = dailyLogs.Where(d => d.Date >= firstDayOfMonth && d.Date <= lastDayOfMonth).ToList();

        var monthlyWeights = monthlyData.Where(d => d.WeightKg.HasValue).Select(d => d.WeightKg!.Value).ToList();
        model.AverageMonthlyWeight = monthlyWeights.Count > 0 ? Math.Round(monthlyWeights.Average(), 2) : null;
        model.ChangeInMonthlyWeight = monthlyWeights.Count > 1 ? monthlyWeights.First() - monthlyWeights.Last() : null;

        var monthlyCalories = monthlyData.Where(d => d.Calories.HasValue).Select(d => d.Calories!.Value).ToList();
        model.AverageMonthlyCalories = monthlyCalories.Count > 0 ? Convert.ToInt32(monthlyCalories.Average()) : null;

        var monthlyDistance = monthlyData.Where(d => d.DistanceKm.HasValue).Select(d => d.DistanceKm!.Value).ToList();
        model.TotalMonthlyDistance = monthlyDistance.Count > 0 ? Math.Round(monthlyDistance.Sum(), 2) : null;

        var monthlySteps = monthlyData.Where(d => d.StepCount.HasValue).Select(d => d.StepCount!.Value).ToList();
        model.TotalMonthlySteps = monthlySteps.Count > 0 ? monthlySteps.Sum() : null;

        // Chart data
        foreach (var sli in model.AvailableDates.OrderBy(sli => sli.Value))
        {
            var date = DateOnly.Parse(sli.Value);

            firstDayOfMonth = date.FirstDayOfMonth();
            lastDayOfMonth = date.LastDayOfMonth();

            monthlyWeights = dailyLogs
                                .Where(d => d.Date >= firstDayOfMonth && d.Date <= lastDayOfMonth && d.WeightKg.HasValue)
                                .Select(d => d.WeightKg!.Value)
                                .ToList();

            if(monthlyWeights.Count > 0)
            {
                model.AverageWeights.Add(Math.Round(monthlyWeights.Average(), 2));
                model.ChartLabels.Add($"{english.DateTimeFormat.GetAbbreviatedMonthName(date.Month)} {date.Year}");
            }
        }

        return View(model);
    }
}
