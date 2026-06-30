using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Entities;

namespace MVC.Controllers;

[Authorize]
public class CoachController : Controller
{
    private readonly ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;

    public CoachController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private async Task<CoachIndexViewModel> CreateCoachIndexViewModel(CoachIndexViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        model.AssignedCoaches = await _context.CoachAssignments
            .Where(ca => ca.MemberId == userId)
            .Select(ca => new AssignedCoachViewModel
            {
                Id = ca.Id,
                UserName = ca.Coach.UserName!
            })
            .ToListAsync();

        return model;
    }

    [HttpGet("coach")]
    public async Task<IActionResult> Index()
    {
        var model = new CoachIndexViewModel();
        model = await CreateCoachIndexViewModel(model);

        return View(model);
    }

    [HttpPost("coach/assign")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign([Bind(Prefix = "AssignCoach")] CoachAssignInputModel model)
    {
        var viewModel = new CoachIndexViewModel { AssignCoach = model };
        viewModel = await CreateCoachIndexViewModel(viewModel);

        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            var coach = await _userManager.FindByNameAsync(model.UserName);

            if (coach == null)
            {
                ModelState.AddModelError("AssignCoach.UserName", "No user with that username exists.");
                return View("Index", viewModel);
            }

            if (userId == coach.Id)
            {
                ModelState.AddModelError("AssignCoach.UserName", "You cannot assign yourself as your own coach.");
                return View("Index", viewModel);
            }

            var isAlreadyAssigned = await _context.CoachAssignments.AnyAsync(ca => ca.MemberId == userId && ca.CoachId == coach.Id);

            if (isAlreadyAssigned)
            {
                ModelState.AddModelError("AssignCoach.UserName", "This coach has already been assigned.");
                return View("Index", viewModel);
            }

            var assignment = new CoachAssignment
            {
                MemberId = userId!,
                CoachId = coach.Id,
            };

            _context.CoachAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View("Index", viewModel);
    }
}
