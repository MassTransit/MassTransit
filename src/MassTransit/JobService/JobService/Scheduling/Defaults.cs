namespace MassTransit.JobService.Scheduling;

using System;


static class Defaults
{
    internal const int FirstYear = 1970;

    internal static readonly int LastYear = DateTime.UtcNow.Year + 100;
}
