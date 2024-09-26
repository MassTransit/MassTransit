#nullable enable
namespace MassTransit.JobService.Scheduling;

using System;
using System.Collections.Generic;
using System.Linq;


public static class TimeZoneUtil
{
    static readonly Dictionary<string, string> TimeZoneIdAliases = new Dictionary<string, string>();

    public static Func<string, TimeZoneInfo?>? CustomResolver = id => null;

    static TimeZoneUtil()
    {
        TimeZoneIdAliases["UTC"] = "Coordinated Universal Time";
        TimeZoneIdAliases["Coordinated Universal Time"] = "UTC";

        TimeZoneIdAliases["Central European Standard Time"] = "CET";
        TimeZoneIdAliases["CET"] = "Central European Standard Time";

        TimeZoneIdAliases["Eastern Standard Time"] = "US/Eastern";
        TimeZoneIdAliases["US/Eastern"] = "Eastern Standard Time";

        TimeZoneIdAliases["Central Standard Time"] = "US/Central";
        TimeZoneIdAliases["US/Central"] = "Central Standard Time";

        TimeZoneIdAliases["US Central Standard Time"] = "US/Indiana-Stark";
        TimeZoneIdAliases["US/Indiana-Stark"] = "US Central Standard Time";

        TimeZoneIdAliases["Mountain Standard Time"] = "US/Mountain";
        TimeZoneIdAliases["US/Mountain"] = "Mountain Standard Time";

        TimeZoneIdAliases["US Mountain Standard Time"] = "US/Arizona";
        TimeZoneIdAliases["US/Arizona"] = "US Mountain Standard Time";

        TimeZoneIdAliases["Pacific Standard Time"] = "US/Pacific";
        TimeZoneIdAliases["US/Pacific"] = "Pacific Standard Time";

        TimeZoneIdAliases["Alaskan Standard Time"] = "US/Alaska";
        TimeZoneIdAliases["US/Alaska"] = "Alaskan Standard Time";

        TimeZoneIdAliases["Hawaiian Standard Time"] = "US/Hawaii";
        TimeZoneIdAliases["US/Hawaii"] = "Hawaiian Standard Time";

        TimeZoneIdAliases["China Standard Time"] = "Asia/Shanghai";
        TimeZoneIdAliases["Asia/Shanghai"] = "China Standard Time";

        TimeZoneIdAliases["Pakistan Standard Time"] = "Asia/Karachi";
        TimeZoneIdAliases["Asia/Karachi"] = "Pakistan Standard Time";
    }

    /// <summary>
    /// TimeZoneInfo.ConvertTime is not supported under mono
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="timeZoneInfo"></param>
    /// <returns></returns>
    public static DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
    {
        return TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo);
    }

    /// <summary>
    /// TimeZoneInfo.GetUtcOffset(DateTimeOffset) is not supported under mono
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="timeZoneInfo"></param>
    /// <returns></returns>
    public static TimeSpan GetUtcOffset(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
    {
        return timeZoneInfo.GetUtcOffset(dateTimeOffset);
    }

    public static TimeSpan GetUtcOffset(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        // Unlike the default behavior of TimeZoneInfo.GetUtcOffset, it is prefered to choose
        // the DAYLIGHT time when the input is ambiguous, because the daylight instance is the
        // FIRST instance, and time moves in a forward direction.

        var offset = timeZoneInfo.IsAmbiguousTime(dateTime)
            ? timeZoneInfo.GetAmbiguousTimeOffsets(dateTime).Max()
            : timeZoneInfo.GetUtcOffset(dateTime);

        return offset;
    }

    /// <summary>
    /// Tries to find time zone with given id, has ability do some fallbacks when necessary.
    /// </summary>
    /// <param name="id">System id of the time zone.</param>
    /// <returns></returns>
    public static TimeZoneInfo FindTimeZoneById(string id)
    {
        TimeZoneInfo? info = null;
        try
        {
            info = TimeZoneInfo.FindSystemTimeZoneById(id);
        }
        catch (TimeZoneNotFoundException ex)
        {
            if (TimeZoneIdAliases.TryGetValue(id, out var aliasedId))
            {
                try
                {
                    info = TimeZoneInfo.FindSystemTimeZoneById(aliasedId);
                }
                catch
                {
                }
            }

            info ??= CustomResolver?.Invoke(id);
            if (info is null)
            {
                throw new TimeZoneNotFoundException(
                    $"Could not find time zone with id {id}, consider using Quartz.Plugins.TimeZoneConverter for resolving more time zones ids", ex);
            }
        }

        return info;
    }
}
