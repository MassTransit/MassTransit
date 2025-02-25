#nullable enable
namespace MassTransit;

using System;


public static class RecurringJobScheduleConfiguratorExtensions
{
    /// <summary>
    /// Sets the cron expression to run daily at the specified hour (and optionally, minute and second)
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static IRecurringJobScheduleConfigurator DailyAt(this IRecurringJobScheduleConfigurator configurator, int hour, int minute = 0, int second = 0)
    {
        ValidateHour(hour);
        ValidateMinute(minute);
        ValidateSecond(second);

        configurator.CronExpression = $"0 {minute} {hour} ? * *";

        return configurator;
    }

    /// <summary>
    /// Sets the cron expression to run on the specified days of the week at the specified hour and minute
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    public static IRecurringJobScheduleConfigurator At(this IRecurringJobScheduleConfigurator configurator, int hour, int minute, params DayOfWeek[] days)
    {
        ValidateHour(hour);
        ValidateMinute(minute);

        if (days is null || days.Length == 0)
            throw new ArgumentException("At least one day of the week must be specified", nameof(days));

        var cronExpression = $"0 {minute} {hour} ? * {(int)days[0] + 1}";

        for (var i = 1; i < days.Length; i++)
            cronExpression = cronExpression + "," + ((int)days[i] + 1);

        configurator.CronExpression = cronExpression;

        return configurator;
    }

    /// <summary>
    /// Sets the cron expression to run on the specified days of the week at the specified hour and minute
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="seconds"></param>
    /// <param name="hours">If specified, job will run every <paramref name="hour" /> hours at <paramref name="minute" />:<paramref name="second" /></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="days"></param>
    /// <param name="hour"></param>
    /// <param name="minutes"></param>
    /// <returns></returns>
    public static IRecurringJobScheduleConfigurator Every(this IRecurringJobScheduleConfigurator configurator, int? hours = default, int? minutes = null,
        int? seconds = null, int? hour = null, int? minute = null, int? second = null, DayOfWeek[]? days = null)
    {
        string? cronExpression;
        if (hours.HasValue)
            cronExpression = $"{second?.ToString() ?? "0"} {minute?.ToString() ?? "0"} {hour?.ToString() ?? "0"}/{hours} * * ";
        else if (minutes.HasValue)
            cronExpression = $"{second?.ToString() ?? "0"} {minute?.ToString() ?? "0"}/{minutes} {hour?.ToString() ?? "*"} * * ";
        else if (seconds.HasValue)
            cronExpression = $"{second?.ToString() ?? "0"}/{seconds} {minute?.ToString() ?? "*"} {hour?.ToString() ?? "*"} * * ";
        else
            throw new ArgumentException("At least one time interval must be specified");

        if (days is { Length: > 0 })
        {
            cronExpression += $"{(int)days[0] + 1}";
            for (var i = 1; i < days.Length; i++)
                cronExpression = cronExpression + "," + ((int)days[i] + 1);
        }
        else
            cronExpression += "*";

        configurator.CronExpression = cronExpression;

        return configurator;
    }

    /// <summary>
    /// Sets the cron expression to run weekly on the specified day at the specified hour and minute
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="dayOfWeek">The day of the week to run</param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <returns></returns>
    public static IRecurringJobScheduleConfigurator Weekly(this IRecurringJobScheduleConfigurator configurator, DayOfWeek dayOfWeek, int hour, int minute = 0)
    {
        ValidateHour(hour);
        ValidateMinute(minute);

        configurator.CronExpression = $"0 {minute} {hour} ? * {(int)dayOfWeek + 1}";

        return configurator;
    }

    /// <summary>
    /// Sets the cron expression to run monthly on the specified day of the month at the specified hour and minute
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="dayOfMonth">The day of the month to run</param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <returns></returns>
    public static IRecurringJobScheduleConfigurator Monthly(this IRecurringJobScheduleConfigurator configurator, int dayOfMonth, int hour, int minute = 0)
    {
        ValidateHour(hour);
        ValidateMinute(minute);
        ValidateDayOfMonth(dayOfMonth);

        configurator.CronExpression = $"0 {minute} {hour} {dayOfMonth} * ?";

        return configurator;
    }

    /// <summary>
    /// Sets the cron expression to run annually on the specified day of the month of the specified month at the specified hour and minute
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="month">The month of the year to run</param>
    /// <param name="dayOfMonth">The day of the month to run</param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <returns></returns>
    public static IRecurringJobScheduleConfigurator Yearly(this IRecurringJobScheduleConfigurator configurator, int month, int dayOfMonth, int hour,
        int minute = 0)
    {
        ValidateMonth(month);
        ValidateHour(hour);
        ValidateMinute(minute);
        ValidateDayOfMonth(dayOfMonth);

        configurator.CronExpression = $"0 {minute} {hour} {dayOfMonth} {month} ?";

        return configurator;
    }

    /// <summary>
    /// Specify the time zone for the cron expression evaluation
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="tz"></param>
    /// <returns></returns>
    public static IRecurringJobScheduleConfigurator SetTimeZone(this IRecurringJobScheduleConfigurator configurator, TimeZoneInfo tz)
    {
        configurator.TimeZoneId = tz.Id;

        return configurator;
    }

    static void ValidateHour(int hour)
    {
        if (hour is < 0 or > 23)
            throw new ArgumentOutOfRangeException(nameof(hour), "Invalid hour (must be >= 0 and <= 23).");
    }

    static void ValidateMinute(int minute)
    {
        if (minute is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(minute), "Invalid minute (must be >= 0 and <= 59).");
    }

    static void ValidateSecond(int second)
    {
        if (second is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(second), "Invalid second (must be >= 0 and <= 59).");
    }

    static void ValidateDayOfMonth(int day)
    {
        if (day is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(day), "Invalid day of month.");
    }

    static void ValidateMonth(int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Invalid month (must be >= 1 and <= 12).");
    }

    static void ValidateYear(int year)
    {
        if (year is < 1970 or > 2099)
            throw new ArgumentOutOfRangeException(nameof(year), "Invalid year (must be >= 1970 and <= 2099).");
    }
}
