namespace MassTransit.JobService.Scheduling;

using System.Globalization;
using System.Text;


readonly struct CronExpressionSummary
{
    public CronExpressionSummary(CronField seconds, CronField minutes, CronField hours, CronField daysOfMonth,
        CronField months, CronField daysOfWeek, bool lastDayOfWeek, bool nearestWeekday, int nthDayOfWeek,
        bool lastDayOfMonth, bool calendarDayOfWeek, bool calendarDayOfMonth, CronField years)
    {
        Seconds = seconds;
        Minutes = minutes;
        Hours = hours;
        DaysOfMonth = daysOfMonth;
        Months = months;
        DaysOfWeek = daysOfWeek;
        LastDayOfWeek = lastDayOfWeek;
        NearestWeekday = nearestWeekday;
        NthDayOfWeek = nthDayOfWeek;
        LastDayOfMonth = lastDayOfMonth;
        CalendarDayOfWeek = calendarDayOfWeek;
        CalendarDayOfMonth = calendarDayOfMonth;
        Years = years;
    }

    public CronField Seconds { get; }
    public CronField Minutes { get; }
    public CronField Hours { get; }
    public CronField DaysOfMonth { get; }
    public CronField Months { get; }
    public CronField DaysOfWeek { get; }
    public bool LastDayOfWeek { get; }
    public bool NearestWeekday { get; }
    public int NthDayOfWeek { get; }
    public bool LastDayOfMonth { get; }
    public bool CalendarDayOfWeek { get; }
    public bool CalendarDayOfMonth { get; }
    public CronField Years { get; }

    /// <summary>
    /// Gets the expression set summary.
    /// </summary>
    static string GetExpressionSetSummary(CronField data)
    {
        if (data.Contains(CronExpressionConstants.NoSpec))
            return "?";

        if (data.Contains(CronExpressionConstants.AllSpec))
            return "*";

        var b = new StringBuilder();

        var first = true;
        foreach (var iVal in data)
        {
            var val = iVal.ToString(CultureInfo.InvariantCulture);
            if (!first)
                b.Append(',');

            b.Append(val);
            first = false;
        }

        return b.ToString();
    }

    public override string ToString()
    {
        var b = new StringBuilder();

        b.Append("seconds: ");
        b.AppendLine(GetExpressionSetSummary(Seconds));
        b.Append("minutes: ");
        b.AppendLine(GetExpressionSetSummary(Minutes));
        b.Append("hours: ");
        b.AppendLine(GetExpressionSetSummary(Hours));
        b.Append("daysOfMonth: ");
        b.AppendLine(GetExpressionSetSummary(DaysOfMonth));
        b.Append("months: ");
        b.AppendLine(GetExpressionSetSummary(Months));
        b.Append("daysOfWeek: ");
        b.AppendLine(GetExpressionSetSummary(DaysOfWeek));
        b.Append("lastdayOfWeek: ");
        b.AppendLine(LastDayOfWeek.ToString());
        b.Append("nearestWeekday: ");
        b.AppendLine(NearestWeekday.ToString());
        b.Append("NthDayOfWeek: ");
        b.AppendLine(NthDayOfWeek.ToString());
        b.Append("lastdayOfMonth: ");
        b.AppendLine(LastDayOfMonth.ToString());
        b.Append("calendardayOfWeek: ");
        b.AppendLine(CalendarDayOfWeek.ToString());
        b.Append("calendardayOfMonth: ");
        b.AppendLine(CalendarDayOfMonth.ToString());
        b.Append("years: ");
        b.AppendLine(GetExpressionSetSummary(Years));
        return b.ToString();
    }
}
