namespace MVC.Models;

public class ClientDailyLogViewModel
{
    public string UserName { get; set; } = default!;
    public List<DailyLogIndexViewModel> DailyLogs { get; set; } = new();
}
