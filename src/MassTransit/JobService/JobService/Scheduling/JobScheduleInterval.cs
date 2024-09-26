#nullable enable
namespace MassTransit.JobService.Scheduling;

using System;


public enum JobScheduleInterval
{
    Second,
    Minute,
    Hour,
    Day,
    Week,
    Month,
    Year
}


public sealed class DateBuilder
{
    int _day;
    int _hour;
    int _minute;
    int _month;
    int _second;
    TimeZoneInfo? _tz;
    int _year;

    /// <summary>
    /// Create a DateBuilder, with initial settings for the current date and time in the given timezone.
    /// </summary>
    DateBuilder(TimeZoneInfo? tz = null)
    {
        if (tz is not null)
            _tz = tz;

        var now = DateTimeOffset.Now.DateTime;

        _month = now.Month;
        _day = now.Day;
        _year = now.Year;
        _hour = now.Hour;
        _minute = now.Minute;
        _second = now.Second;
    }

    /// <summary>
    /// Build the <see cref="System.DateTimeOffset" /> defined by this builder instance.
    /// </summary>
    /// <returns>New date time based on builder parameters.</returns>
    public DateTimeOffset Build()
    {
        var dt = new DateTime(_year, _month, _day, _hour, _minute, _second);
        var offset = TimeZoneUtil.GetUtcOffset(dt, _tz ?? TimeZoneInfo.Local);
        return new DateTimeOffset(dt, offset);
    }

    /// <summary>
    /// Set the hour (0-23) for the Date that will be built by this builder.
    /// </summary>
    /// <param name="hour"></param>
    /// <returns></returns>
    public DateBuilder AtHourOfDay(int hour)
    {
        ValidateHour(hour);

        _hour = hour;
        return this;
    }

    /// <summary>
    /// Set the minute (0-59) for the Date that will be built by this builder.
    /// </summary>
    /// <param name="minute"></param>
    /// <returns></returns>
    public DateBuilder AtMinute(int minute)
    {
        ValidateMinute(minute);

        _minute = minute;
        return this;
    }

    /// <summary>
    /// Set the second (0-59) for the Date that will be built by this builder, and truncate the milliseconds to 000.
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public DateBuilder AtSecond(int second)
    {
        ValidateSecond(second);

        _second = second;
        return this;
    }

    public DateBuilder AtHourMinuteAndSecond(int hour, int minute, int second)
    {
        ValidateHour(hour);
        ValidateMinute(minute);
        ValidateSecond(second);

        _hour = hour;
        _second = second;
        _minute = minute;
        return this;
    }

    /// <summary>
    /// Set the day of month (1-31) for the Date that will be built by this builder.
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public DateBuilder OnDay(int day)
    {
        ValidateDayOfMonth(day);

        _day = day;
        return this;
    }

    /// <summary>
    /// Set the month (1-12) for the Date that will be built by this builder.
    /// </summary>
    /// <param name="month"></param>
    /// <returns></returns>
    public DateBuilder InMonth(int month)
    {
        ValidateMonth(month);

        _month = month;
        return this;
    }

    public DateBuilder InMonthOnDay(int month, int day)
    {
        ValidateMonth(month);
        ValidateDayOfMonth(day);

        _month = month;
        _day = day;
        return this;
    }

    /// <summary>
    /// Set the year for the Date that will be built by this builder.
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public DateBuilder InYear(int year)
    {
        ValidateYear(year);

        _year = year;
        return this;
    }

    /// <summary>
    /// Set the TimeZoneInfo for the Date that will be built by this builder (if "null", system default will be used)
    /// </summary>
    /// <param name="tz"></param>
    /// <returns></returns>
    public DateBuilder InTimeZone(TimeZoneInfo tz)
    {
        _tz = tz;
        return this;
    }

    public static DateTimeOffset FutureDate(int interval, JobScheduleInterval unit)
    {
        return TranslatedAdd(DateTimeOffset.Now, unit, interval);
    }

    /// <summary>
    /// Get a <see cref="System.DateTimeOffset" /> object that represents the given time, on tomorrow's date.
    /// </summary>
    public static DateTimeOffset TomorrowAt(int hour, int minute, int second)
    {
        ValidateSecond(second);
        ValidateMinute(minute);
        ValidateHour(hour);

        var now = DateTimeOffset.Now;
        var c = new DateTimeOffset(
            now.Year,
            now.Month,
            now.Day,
            hour,
            minute,
            second,
            0,
            now.Offset);

        // advance one day
        c = c.AddDays(1);

        return c;
    }

    /// <summary>
    /// Get a <see cref="DateTimeOffset" /> object that represents the given time, on today's date
    /// </summary>
    public static DateTimeOffset TodayAt(int hour, int minute, int second)
    {
        return DateOf(hour, minute, second);
    }

    static DateTimeOffset TranslatedAdd(DateTimeOffset date, JobScheduleInterval unit, int amountToAdd)
    {
        switch (unit)
        {
            case JobScheduleInterval.Day:
                return date.AddDays(amountToAdd);
            case JobScheduleInterval.Hour:
                return date.AddHours(amountToAdd);
            case JobScheduleInterval.Minute:
                return date.AddMinutes(amountToAdd);
            case JobScheduleInterval.Month:
                return date.AddMonths(amountToAdd);
            case JobScheduleInterval.Second:
                return date.AddSeconds(amountToAdd);
            case JobScheduleInterval.Week:
                return date.AddDays(amountToAdd * 7);
            case JobScheduleInterval.Year:
                return date.AddYears(amountToAdd);
            default:
                throw new ArgumentException("Invalid job schedule interval", nameof(unit));
        }
    }

    /// <summary>
    /// Get a <see cref="DateTimeOffset" /> object that represents the given time, on today's date.
    /// </summary>
    /// <param name="second">The value (0-59) to give the seconds field of the date</param>
    /// <param name="minute">The value (0-59) to give the minutes field of the date</param>
    /// <param name="hour">The value (0-23) to give the hours field of the date</param>
    /// <returns>the new date</returns>
    public static DateTimeOffset DateOf(int hour, int minute, int second)
    {
        ValidateSecond(second);
        ValidateMinute(minute);
        ValidateHour(hour);

        var c = DateTimeOffset.Now;
        var dt = new DateTime(c.Year, c.Month, c.Day, hour, minute, second);
        return new DateTimeOffset(dt, TimeZoneUtil.GetUtcOffset(dt, TimeZoneInfo.Local));
    }

    /// <summary>
    /// Get a <see cref="DateTimeOffset" /> object that represents the given time, on the
    /// given date.
    /// </summary>
    /// <param name="second">The value (0-59) to give the seconds field of the date</param>
    /// <param name="minute">The value (0-59) to give the minutes field of the date</param>
    /// <param name="hour">The value (0-23) to give the hours field of the date</param>
    /// <param name="dayOfMonth">The value (1-31) to give the day of month field of the date</param>
    /// <param name="month">The value (1-12) to give the month field of the date</param>
    /// <returns>the new date</returns>
    public static DateTimeOffset DateOf(int hour, int minute, int second, int dayOfMonth, int month)
    {
        ValidateSecond(second);
        ValidateMinute(minute);
        ValidateHour(hour);
        ValidateDayOfMonth(dayOfMonth);
        ValidateMonth(month);

        var c = DateTimeOffset.Now;
        var dt = new DateTime(c.Year, month, dayOfMonth, hour, minute, second);
        return new DateTimeOffset(dt, TimeZoneUtil.GetUtcOffset(dt, TimeZoneInfo.Local));
    }

    /// <summary>
    /// Get a <see cref="System.DateTimeOffset" /> object that represents the given time, on the
    /// given date.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="second">The value (0-59) to give the seconds field of the date</param>
    /// <param name="minute">The value (0-59) to give the minutes field of the date</param>
    /// <param name="hour">The value (0-23) to give the hours field of the date</param>
    /// <param name="dayOfMonth">The value (1-31) to give the day of month field of the date</param>
    /// <param name="month">The value (1-12) to give the month field of the date</param>
    /// <param name="year">The value (1970-2099) to give the year field of the date</param>
    /// <returns>the new date</returns>
    public static DateTimeOffset DateOf(int hour, int minute, int second, int dayOfMonth, int month, int year)
    {
        ValidateSecond(second);
        ValidateMinute(minute);
        ValidateHour(hour);
        ValidateDayOfMonth(dayOfMonth);
        ValidateMonth(month);
        ValidateYear(year);

        var dt = new DateTime(year, month, dayOfMonth, hour, minute, second);
        return new DateTimeOffset(dt, TimeZoneUtil.GetUtcOffset(dt, TimeZoneInfo.Local));
    }

    /// <summary>
    /// Returns a date that is rounded to the next even hour after the current time.
    /// </summary>
    /// <remarks>
    /// For example a current time of 08:13:54 would result in a date
    /// with the time of 09:00:00. If the date's time is in the 23rd hour, the
    /// date's 'day' will be promoted, and the time will be set to 00:00:00.
    /// </remarks>
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenHourDateAfterNow()
    {
        return EvenHourDate(null);
    }

    /// <summary>
    /// Returns a date that is rounded to the next even hour above the given date.
    /// </summary>
    /// <remarks>
    /// For example an input date with a time of 08:13:54 would result in a date
    /// with the time of 09:00:00. If the date's time is in the 23rd hour, the
    /// date's 'day' will be promoted, and the time will be set to 00:00:00.
    /// </remarks>
    /// <param name="date">
    /// the Date to round, if <see langword="null" /> the current time will
    /// be used
    /// </param>
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenHourDate(DateTimeOffset? date)
    {
        date ??= DateTimeOffset.Now;

        var d = date.Value.AddHours(1);
        return new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, 0, 0, d.Offset);
    }

    /// <summary>
    /// Returns a date that is rounded to the previous even hour below the given date.
    /// </summary>
    /// <remarks>
    /// For example an input date with a time of 08:13:54 would result in a date
    /// with the time of 08:00:00.
    /// </remarks>
    /// <param name="date">the Date to round, if <see langword="null" /> the current time will be used</param>
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenHourDateBefore(DateTimeOffset? date)
    {
        date ??= DateTimeOffset.Now;

        return new DateTimeOffset(date.Value.Year, date.Value.Month, date.Value.Day, date.Value.Hour, 0, 0, date.Value.Offset);
    }

    /// <summary>
    /// <para>
    /// Returns a date that is rounded to the next even minute after the current time.
    /// </para>
    /// </summary>
    /// <remarks>
    /// For example a current time of 08:13:54 would result in a date
    /// with the time of 08:14:00. If the date's time is in the 59th minute,
    /// then the hour (and possibly the day) will be promoted.
    /// </remarks>
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenMinuteDateAfterNow()
    {
        return EvenMinuteDate(DateTimeOffset.Now);
    }

    /// <summary>
    /// Returns a date that is rounded to the next even minute above the given date.
    /// </summary>
    /// <remarks>
    /// For example an input date with a time of 08:13:54 would result in a date
    /// with the time of 08:14:00. If the date's time is in the 59th minute,
    /// then the hour (and possibly the day) will be promoted.
    /// </remarks>
    /// <param name="date">The Date to round, if <see langword="null" /> the current time will  be used</param>
    /// <returns>The new rounded date</returns>
    public static DateTimeOffset EvenMinuteDate(DateTimeOffset? date)
    {
        date ??= DateTimeOffset.Now;

        var d = date.Value;
        d = d.AddMinutes(1);
        return new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0, d.Offset);
    }

    /// <summary>
    /// Returns a date that is rounded to the previous even minute below the given date.
    /// </summary>
    /// <remarks>
    /// For example an input date with a time of 08:13:54 would result in a date
    /// with the time of 08:13:00.
    /// </remarks>
    /// <param name="date">the Date to round, if <see langword="null" /> the current time will be used</param>
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenMinuteDateBefore(DateTimeOffset? date)
    {
        date ??= DateTimeOffset.Now;

        var d = date.Value;
        return new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0, d.Offset);
    }

    /// <summary>
    /// Returns a date that is rounded to the next even second after the current time.
    /// </summary>
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenSecondDateAfterNow()
    {
        return EvenSecondDate(DateTimeOffset.Now);
    }

    /// <summary>
    /// Returns a date that is rounded to the next even second above the given date.
    /// </summary>
    /// <param name="date"></param>
    /// the Date to round, if
    /// <see langword="null" />
    /// the current time will
    /// be used
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenSecondDate(DateTimeOffset date)
    {
        date = date.AddSeconds(1);
        return new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0, date.Offset);
    }

    /// <summary>
    /// Returns a date that is rounded to the previous even second below the
    /// given date.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For example an input date with a time of 08:13:54.341 would result in a
    /// date with the time of 08:13:00.000.
    /// </para>
    /// </remarks>
    /// <param name="date"></param>
    /// the Date to round, if
    /// <see langword="null" />
    /// the current time will
    /// be used
    /// <returns>the new rounded date</returns>
    public static DateTimeOffset EvenSecondDateBefore(DateTimeOffset date)
    {
        return new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0, date.Offset);
    }

    public static DateTimeOffset NextGivenMinuteDate(DateTimeOffset? date, int minuteBase)
    {
        if (minuteBase < 0 || minuteBase > 59)
            throw new ArgumentOutOfRangeException(nameof(minuteBase), "must be >= 0 and <= 59");

        var c = date ?? DateTimeOffset.Now;

        if (minuteBase == 0)
            return new DateTimeOffset(c.Year, c.Month, c.Day, c.Hour, 0, 0, 0, c.Offset).AddHours(1);

        var minute = c.Minute;

        var arItr = minute / minuteBase;

        var nextMinuteOccurence = minuteBase * (arItr + 1);

        if (nextMinuteOccurence < 60)
            return new DateTimeOffset(c.Year, c.Month, c.Day, c.Hour, nextMinuteOccurence, 0, 0, c.Offset);
        return new DateTimeOffset(c.Year, c.Month, c.Day, c.Hour, 0, 0, 0, c.Offset).AddHours(1);
    }

    public static DateTimeOffset NextGivenSecondDate(DateTimeOffset? date, int secondBase)
    {
        if (secondBase < 0 || secondBase > 59)
            throw new ArgumentOutOfRangeException(nameof(secondBase), "must be >= 0 and <= 59");

        var c = date ?? DateTimeOffset.Now;

        if (secondBase == 0)
            return new DateTimeOffset(c.Year, c.Month, c.Day, c.Hour, c.Minute, 0, 0, c.Offset).AddMinutes(1);

        var second = c.Second;

        var arItr = second / secondBase;

        var nextSecondOccurence = secondBase * (arItr + 1);

        if (nextSecondOccurence < 60)
            return new DateTimeOffset(c.Year, c.Month, c.Day, c.Hour, c.Minute, nextSecondOccurence, 0, c.Offset);
        return new DateTimeOffset(c.Year, c.Month, c.Day, c.Hour, c.Minute, 0, 0, c.Offset).AddMinutes(1);
    }

    internal static void ValidateHour(int hour)
    {
        if (hour is < 0 or > 23)
            throw new ArgumentOutOfRangeException(nameof(hour), "Invalid hour (must be >= 0 and <= 23).");
    }

    internal static void ValidateMinute(int minute)
    {
        if (minute is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(minute), "Invalid minute (must be >= 0 and <= 59).");
    }

    static void ValidateSecond(int second)
    {
        if (second is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(second), "Invalid second (must be >= 0 and <= 59).");
    }

    internal static void ValidateDayOfMonth(int day)
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
