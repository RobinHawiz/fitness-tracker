using MVC.Models;

namespace MVC.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(string userId, string? selectedDate);
}
