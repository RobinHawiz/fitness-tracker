namespace MVC.Models;

public class ClientDashboardViewModel
{
    public string UserName { get; set; } = default!;
    public DashboardViewModel Dashboard { get; set; } = null!;
}
