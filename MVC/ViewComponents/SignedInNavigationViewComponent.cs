using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models.Entities;

namespace MVC.ViewComponents;

public class SignedInNavigationViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;

    public SignedInNavigationViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View(await HasClientsAsync());
    }

    private async Task<bool> HasClientsAsync()
    {
        var userId = _userManager.GetUserId(HttpContext.User);

        if (userId == null)
        {
            return false;
        }

        return await _context.CoachAssignments
          .AnyAsync(ca => ca.CoachId == userId);
    }
}
