
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Entities;

[Authorize]
public class DailyLogsController : Controller
{
    private readonly ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;

    public DailyLogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: dailylogs
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var dailyLogs = await _context.DailyLogs
                        .Where(d => d.ApplicationUserId == userId!)
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

        return View(dailyLogs);
    }

    // GET: dailylogs/Create
    public IActionResult Create()
    {
        var model = new DailyLogFormViewModel
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
        };
        return View(model);
    }

    // POST: dailylogs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DailyLogFormViewModel dailyLogFormViewModel)
    {
        var userId = _userManager.GetUserId(User);
        if (ModelState.IsValid)
        {
            var existingLog = await _context.DailyLogs
                .FirstOrDefaultAsync(d => d.ApplicationUserId == userId && d.Date == dailyLogFormViewModel.Date);
            if (existingLog != null)
            {
                ModelState.AddModelError("Date", "A daily log for this date already exists.");
                return View(dailyLogFormViewModel);
            }

            var dailylog = new DailyLog
            {
                Date = dailyLogFormViewModel.Date,
                WeightKg = dailyLogFormViewModel.WeightKg,
                ApplicationUserId = userId!,

                DailyMacros = new DailyMacros
                {
                    Fat = dailyLogFormViewModel.Fat,
                    Carbs = dailyLogFormViewModel.Carbs,
                    Protein = dailyLogFormViewModel.Protein,
                    Calories = dailyLogFormViewModel.Calories
                },

                DailyMovement = new DailyMovement
                {
                    DistanceKm = dailyLogFormViewModel.DistanceKm,
                    StepCount = dailyLogFormViewModel.StepCount
                }
            };
            _context.Add(dailylog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(dailyLogFormViewModel);
    }


    // GET: dailylogs/:id
    [HttpGet("dailylogs/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var dailyLog = await _context.DailyLogs
                        .Where(d => d.ApplicationUserId == userId! && d.Id == id)
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
                        }).FirstOrDefaultAsync();

        if (dailyLog == null)
        {
            return NotFound();
        }

        return View(dailyLog);
    }

    // GET: dailylogs/Edit/:id
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var dailyLog = await _context.DailyLogs
                        .Where(d => d.ApplicationUserId == userId! && d.Id == id)
                        .Select(d => new DailyLogFormViewModel
                        {
                            Date = d.Date,
                            WeightKg = d.WeightKg,

                            Fat = d.DailyMacros.Fat,
                            Carbs = d.DailyMacros.Carbs,
                            Protein = d.DailyMacros.Protein,
                            Calories = d.DailyMacros.Calories,

                            DistanceKm = d.DailyMovement.DistanceKm,
                            StepCount = d.DailyMovement.StepCount
                        }).FirstOrDefaultAsync();

        if (dailyLog == null)
        {
            return NotFound();
        }

        return View(dailyLog);
    }

    // POST: dailylogs/Edit/:id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DailyLogFormViewModel dailyLogFormViewModel)
    {

        if (ModelState.IsValid)
        {

            var userId = _userManager.GetUserId(User);

            var dailyLog = await _context.DailyLogs
                .Include(d => d.DailyMacros)
                .Include(d => d.DailyMovement)
                .FirstOrDefaultAsync(d => d.ApplicationUserId == userId && d.Id == id);

            if (dailyLog == null)
            {
                return NotFound();
            }

            dailyLog.WeightKg = dailyLogFormViewModel.WeightKg;

            dailyLog.DailyMacros.Fat = dailyLogFormViewModel.Fat;
            dailyLog.DailyMacros.Carbs = dailyLogFormViewModel.Carbs;
            dailyLog.DailyMacros.Protein = dailyLogFormViewModel.Protein;
            dailyLog.DailyMacros.Calories = dailyLogFormViewModel.Calories;

            dailyLog.DailyMovement.DistanceKm = dailyLogFormViewModel.DistanceKm;
            dailyLog.DailyMovement.StepCount = dailyLogFormViewModel.StepCount;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(dailyLogFormViewModel);
    }
}