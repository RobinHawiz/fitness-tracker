using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Entities;

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
