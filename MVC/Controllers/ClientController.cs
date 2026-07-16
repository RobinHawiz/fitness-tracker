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

[Authorize]
public class ClientController : Controller
{
    private readonly ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;

    public ClientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("client")]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var assignedClients = await _context.CoachAssignments
            .Where(ca => ca.CoachId == userId)
            .Select(ca => new AssignedClientViewModel
            {
                Id = ca.Id,
                UserName = ca.Member.UserName!
            }).ToListAsync();

        return View(assignedClients);
    }

    [HttpGet("client/{id:int}/dailylogs")]
    public async Task<IActionResult> DailyLogs(int id)
    {
        var userId = _userManager.GetUserId(User);

        var coachAssignment = await _context.CoachAssignments
        .Where(ca => ca.CoachId == userId && ca.Id == id)
        .Select(ca => new
        {
            ca.MemberId,
            ca.Member.UserName
        })
        .FirstOrDefaultAsync();

        if (coachAssignment == null)
        {
            return NotFound();
        }

        var dailyLogs = await _context.DailyLogs
                        .Where(d => d.ApplicationUserId == coachAssignment.MemberId)
                        .OrderByDescending(d => d.Date)
                        .Select(d => new DailyLogIndexViewModel
                        {
                            Id = d.Id,
                            Date = d.Date,
                            WeightKg = d.WeightKg,

                            Fat = d.DailyMacros.Fat,
                            Carbs = d.DailyMacros.Carbs,
                            Protein = d.DailyMacros.Protein,
                            Calories = d.DailyMacros.Calories,

                            DistanceKm = d.DailyMovement.DistanceKm,
                            StepCount = d.DailyMovement.StepCount
                        })
                        .ToListAsync();

        var viewModel = new ClientDailyLogViewModel
        {
            UserName = coachAssignment.UserName!,
            DailyLogs = dailyLogs
        };

        return View(viewModel);
    }


    [Authorize]
    [HttpGet("client/{id:int}/dashboard")]
    public async Task<IActionResult> Dashboard(int id, string? selectedDate)
    {
        var userId = _userManager.GetUserId(User);

        var coachAssignment = await _context.CoachAssignments
                            .Where(ca => ca.CoachId == userId && ca.Id == id)
                            .Select(ca => new
                            {
                                ca.MemberId,
                                ca.Member.UserName
                            })
                            .FirstOrDefaultAsync();

        if (coachAssignment == null)
        {
            return NotFound();
        }

        var dailyLogs = await _context.DailyLogs
                .Where(d => d.ApplicationUserId == coachAssignment.MemberId)
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

        var model = new ClientDashboardViewModel
        {
            UserName = coachAssignment.UserName!
        };
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

        if (model.AvailableDates.Count == 0)
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

        // Monthly data
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

        return View(model);
    }

    [HttpPost("client/{id:int}/remove")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = _userManager.GetUserId(User);

        var assignedClient = await _context.CoachAssignments
            .FirstOrDefaultAsync(d => d.CoachId == userId && d.Id == id);

        if (assignedClient == null)
        {
            return NotFound();
        }

        _context.CoachAssignments.Remove(assignedClient);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
