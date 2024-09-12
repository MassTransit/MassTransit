namespace MassTransit.JobService.Scheduling;

using System;
using System.Collections.Generic;


static class SortedSetExtensions
{
    internal static bool TryGetMinValueStartingFrom(this SortedSet<int> set, DateTimeOffset start, bool allowValueBeforeStartDay, out int minimumDay)
    {
        minimumDay = set.Min;
        var startDay = start.Day;

        if (set.Contains(CronExpressionConstants.AllSpec) || set.Contains(startDay))
        {
            minimumDay = startDay;
            return true;
        }

        if (allowValueBeforeStartDay && set.Min < startDay)
            return true;

        if (set.Count == 0 || set.Max < startDay)
            return false;

        if (set.Min >= startDay)
            return true;

        SortedSet<int> view = set.GetViewBetween(startDay, int.MaxValue);
        if (view.Count > 0)
        {
            minimumDay = view.Min;
            return true;
        }

        return false;
    }
}
