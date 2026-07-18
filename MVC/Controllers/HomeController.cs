using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Entities;
using MVC.Services;

namespace MVC.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IDashboardService _dashboardService;
    public HomeController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IDashboardService dashboardService)
    {
        _context = context;
        _signInManager = signInManager;
        _dashboardService = dashboardService;
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
        var model = await _dashboardService.GetDashboardAsync(userId!, selectedDate);

        return View(model);
    }
}
