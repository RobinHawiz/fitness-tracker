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
            return RedirectToAction("Index", "DailyLogs");
        }

        var model = new HomeIndexViewModel
        {
            UserNames = await _context.Users.Select(t => t.UserName!).ToListAsync(),
        };

        return View(model);
    }
}
