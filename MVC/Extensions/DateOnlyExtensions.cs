namespace MVC.Extensions;

public static class DateOnlyExtensions
{
    public static DateOnly FirstDayOfWeek(this DateOnly dt)
    {
        // Assume Monday is the first day of the week.
        // Sunday enum value is 0, so we set it to 6 to get the correct difference.
        var diff = dt.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)dt.DayOfWeek - (int)DayOfWeek.Monday;

        return dt.AddDays(-diff);
    }

    public static DateOnly LastDayOfWeek(this DateOnly dt) =>
        dt.FirstDayOfWeek().AddDays(6);

    public static DateOnly FirstDayOfMonth(this DateOnly dt) =>
        new(dt.Year, dt.Month, 1);

    public static DateOnly LastDayOfMonth(this DateOnly dt) =>
        dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

    public static DateOnly FirstDayOfYear(this DateOnly dt) =>
        new(dt.Year, 1, 1);

    public static DateOnly LastDayOfYear(this DateOnly dt) =>
        new(dt.Year, 12, 31);
}
