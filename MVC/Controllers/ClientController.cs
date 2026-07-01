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
