#nullable enable
namespace MassTransit.JobService.Scheduling;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Internals;


public sealed class CronExpression :
    IEquatable<CronExpression>
{
    static readonly Regex _regex = new(@"^L(-\d{1,2})?(W(-\d{1,2})?)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));
    static readonly Regex _offsetRegex = new("LW-(?<offset>[0-9]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));

    readonly CronField _daysOfMonth = [];
    readonly CronField _daysOfWeek = [];
    readonly CronField _hours = [];
    readonly CronField _minutes = [];
    readonly CronField _months = [];
    readonly CronField _seconds = [];
    readonly CronField _years = [];

    bool _calendarDayOfMonth;
    bool _calendarDayOfWeek;
    int _everyNthWeek;
    int _lastDayOffset;
    bool _lastDayOfMonth;
    bool _lastDayOfWeek;
    int _lastWeekdayOffset;
    bool _nearestWeekday;
    int _nthDayOfWeek;
    TimeZoneInfo? _timeZone;

    static CronExpression()
    {
    }

    public CronExpression(string? cronExpression)
    {
        if (cronExpression is null)
            throw new ArgumentNullException(nameof(cronExpression));

        CronExpressionString = CultureInfo.InvariantCulture.TextInfo.ToUpper(cronExpression).Trim();

        BuildExpression(CronExpressionString);
    }

    public TimeZoneInfo TimeZone
    {
        set => _timeZone = value;
        get => _timeZone ??= TimeZoneInfo.Local;
    }

    string CronExpressionString { get; }

    public bool Equals(CronExpression? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Equals(_timeZone, other._timeZone) && CronExpressionString == other.CronExpressionString;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is CronExpression other && Equals(other));
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((_timeZone != null ? _timeZone.GetHashCode() : 0) * 397) ^ CronExpressionString.GetHashCode();
        }
    }

    public static bool operator ==(CronExpression? left, CronExpression? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CronExpression? left, CronExpression? right)
    {
        return !Equals(left, right);
    }

    public bool IsSatisfiedBy(DateTimeOffset date)
    {
        var withoutMilliseconds = new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Offset);
        var test = withoutMilliseconds.AddSeconds(-1);
        DateTimeOffset? timeAfter = GetTimeAfter(test);

        return timeAfter.HasValue && timeAfter.Value.Equals(withoutMilliseconds);
    }

    public DateTimeOffset? GetNextValidTimeAfter(DateTimeOffset date)
    {
        return GetTimeAfter(date);
    }

    public override string ToString()
    {
        return CronExpressionString;
    }

    public static bool IsValidExpression(string cronExpression)
    {
        try
        {
            _ = new CronExpression(cronExpression);
        }
        catch (FormatException)
        {
            return false;
        }

        return true;
    }

    public static void ValidateExpression(string cronExpression)
    {
        _ = new CronExpression(cronExpression);
    }

    void BuildExpression(string expression)
    {
        try
        {
            ClearExpressionFields();

            var index = CronExpressionConstants.Second;

            foreach ((ReadOnlySpan<char> expr, ReadOnlySpan<char> _) in expression.SpanSplit(' ', '\t'))
            {
                if (index > CronExpressionConstants.Year)
                    break;

                if (index == CronExpressionConstants.DayOfMonth)
                {
                    if (expr.IndexOf('L') != -1 && expr.Length > 1 && expr.IndexOf(',') >= 0 && expr.Slice(expr.IndexOf('L') + 1).IndexOf('L') != -1)
                        throw new FormatException("Support for specifying 'L' with other days of the month is limited to one instance of L");
                }

                if (index == CronExpressionConstants.DayOfWeek && expr.IndexOf('L') != -1 && expr.Length > 1 && expr.IndexOf(',') >= 0)
                    throw new FormatException("Support for specifying 'L' with other days of the week is not implemented");

                if (index == CronExpressionConstants.DayOfWeek && expr.IndexOf('#') != -1 && expr.Slice(expr.IndexOf('#') + 1 + 1).IndexOf('#') != -1)
                    throw new FormatException("Support for specifying multiple \"nth\" days is not implemented.");

                if (expr.IndexOf(',') != -1)
                {
                    foreach (var v in expr.SpanSplit(','))
                        StoreExpressionValues(0, v, index);
                }
                else
                    StoreExpressionValues(0, expr, index);

                index++;
            }

            if (index <= CronExpressionConstants.DayOfWeek)
                throw new FormatException("Unexpected end of expression.");

            if (index <= CronExpressionConstants.Year)
                StoreExpressionValues(0, "*".AsSpan(), CronExpressionConstants.Year);
        }
        catch (FormatException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new FormatException($"Illegal cron expression format ({e.Message})", e);
        }
    }

    void ClearExpressionFields()
    {
        _seconds.Clear();
        _minutes.Clear();
        _hours.Clear();
        _daysOfMonth.Clear();
        _months.Clear();
        _daysOfWeek.Clear();
        _years.Clear();
    }

    void StoreExpressionQuestionMark(int type, ReadOnlySpan<char> span, int index)
    {
        index++;
        if (index + 1 <= span.Length && !char.IsWhiteSpace(span[index]))
            throw new FormatException("Illegal character after '?': " + span[index]);

        if (type != CronExpressionConstants.DayOfWeek && type != CronExpressionConstants.DayOfMonth)
            throw new FormatException("'?' can only be specified for Day-of-Month or Day-of-Week.");

        if (type == CronExpressionConstants.DayOfWeek && !_lastDayOfMonth)
        {
            var val = _daysOfMonth.LastOrDefault();
            if (val == CronExpressionConstants.NoSpec)
                throw new FormatException("'?' can only be specified for Day-of-Month -OR- Day-of-Week.");
        }

        AddToSet(CronExpressionConstants.NoSpec, -1, 0, type);
    }

    void StoreExpressionStarOrSlash(int type, ReadOnlySpan<char> span, int index)
    {
        var ch = span[index];
        var incr = 0;
        var startsWithAsterisk = ch == '*';
        if (startsWithAsterisk && index + 1 >= span.Length)
        {
            AddToSet(CronExpressionConstants.AllSpec, -1, incr, type);
            return;
        }

        if (ch == '/' && (index + 1 >= span.Length || char.IsWhiteSpace(span[index + 1])))
            throw new FormatException("'/' must be followed by an integer.");

        if (startsWithAsterisk)
            index++;

        ch = span[index];
        if (ch == '/')
        {
            index++;
            if (index >= span.Length)
                throw new FormatException("Unexpected end of string.");

            incr = GetNumericValue(span, index);
            CheckIncrementRange(incr, type);
        }
        else
        {
            if (startsWithAsterisk)
                throw new FormatException("Illegal characters after asterisk: " + span.ToString());

            incr = 1;
        }

        AddToSet(CronExpressionConstants.AllSpec, -1, incr, type);
    }

    void StoreExpressionL(int type, ReadOnlySpan<char> span, int index)
    {
        index++;
        switch (type)
        {
            case CronExpressionConstants.DayOfMonth:
            {
                _lastDayOfMonth = true;
                if (span.Length > index)
                {
                    var ch = span[index];
                    if (ch == '-')
                    {
                        (_lastDayOffset, index) = GetValue(0, span, index + 1);
                        if (_lastDayOffset > 30)
                            throw new FormatException("Offset from last day must be <= 30");
                    }

                    if (span.Length > index)
                    {
                        ch = span[index];
                        if (ch == 'W')
                            _nearestWeekday = true;

                        var match = _offsetRegex.Match(span.ToString());
                        if (match.Success)
                        {
                            var offSetGroup = match.Groups["offset"];
                            if (offSetGroup.Success)
                                _lastWeekdayOffset = int.Parse(offSetGroup.Value);
                        }
                    }
                }

                break;
            }

            case CronExpressionConstants.DayOfWeek:
                AddToSet(7, 7, 0, type);
                break;

            default:
                throw new FormatException($"'L' option is not valid here. (pos={index})");
        }
    }

    void StoreExpressionNumeric(int type, ReadOnlySpan<char> span, int index)
    {
    #if NET6_0_OR_GREATER
        if (int.TryParse(span, out var temp))
    #else
        if (int.TryParse(span.ToString(), out var temp))
    #endif
        {
            AddToSet(temp, -1, -1, type);
            return;
        }

        var ch = span[index];
        var value = ToInt32(ch);
        index++;
        if (index >= span.Length)
            AddToSet(value, -1, -1, type);
        else
        {
            ch = span[index];
            if (char.IsDigit(ch))
                (value, index) = GetValue(value, span, index);

            CheckNext(index, span, value, type);
        }
    }

    void StoreExpressionGeneralValue(int type, ReadOnlySpan<char> span, int index)
    {
        var incr = 0;
        ReadOnlySpan<char> sub = span.Slice(index, 3);
        int sval;
        var eval = -1;
        if (type == CronExpressionConstants.Month)
        {
            sval = GetMonthNumber(sub) + 1;
            if (sval <= 0)
                throw new FormatException($"Invalid Month value: '{sub.ToString()}'");

            if (span.Length > index + 3)
            {
                if (span[index + 3] == '-')
                {
                    index += 4;
                    sub = span.Slice(index, 3);
                    eval = GetMonthNumber(sub) + 1;
                    if (eval <= 0)
                        throw new FormatException($"Invalid Month value: '{sub.ToString()}'");
                }
            }
        }
        else if (type == CronExpressionConstants.DayOfWeek)
        {
            sval = GetDayOfWeekNumber(sub);
            if (sval < 0)
                throw new FormatException($"Invalid Day-of-Week value: '{sub.ToString()}'");

            if (span.Length > index + 3)
            {
                var c = span[index + 3];
                switch (c)
                {
                    case '-':
                        index += 4;
                        sub = span.Slice(index, 3);
                        eval = GetDayOfWeekNumber(sub);
                        if (eval < 0)
                            throw new FormatException($"Invalid Day-of-Week value: '{sub.ToString()}'");

                        break;
                    case '#':
                        try
                        {
                            index += 4;
                            _nthDayOfWeek = ToInt32(span.Slice(index));
                            if (_nthDayOfWeek is < 1 or > 5)
                                throw new FormatException("nthDayOfWeek is < 1 or > 5");
                        }
                        catch (Exception)
                        {
                            throw new FormatException("A numeric value between 1 and 5 must follow the '#' option");
                        }

                        break;
                    case '/':
                        try
                        {
                            index += 4;
                            _everyNthWeek = ToInt32(span.Slice(index));
                            if (_everyNthWeek is < 1 or > 5)
                                throw new FormatException("everyNthWeek is < 1 or > 5");
                        }
                        catch (Exception)
                        {
                            throw new FormatException("A numeric value between 1 and 5 must follow the '/' option");
                        }

                        break;
                    case 'L':
                        _lastDayOfWeek = true;
                        break;
                    default:
                        throw new FormatException($"Illegal characters for this position: '{sub.ToString()}'");
                }
            }
        }
        else
            throw new FormatException($"Illegal characters for this position: '{sub.ToString()}'");

        if (eval != -1)
            incr = 1;

        AddToSet(sval, eval, incr, type);
    }

    void StoreExpressionValues(int position, ReadOnlySpan<char> span, int type)
    {
        var index = position;
        if (index < span.Length && char.IsWhiteSpace(span[index]))
            index = SkipWhiteSpace(position, span);

        if (index >= span.Length)
            return;

        switch (span[index])
        {
            case >= 'A' and <= 'Z' when !span.SequenceEqual("L".AsSpan()) && !_regex.IsMatch(span.ToString()):
                StoreExpressionGeneralValue(type, span, index);
                break;

            case '?':
                StoreExpressionQuestionMark(type, span, index);
                break;

            case '*':
            case '/':
                StoreExpressionStarOrSlash(type, span, index);
                break;

            case 'L':
                StoreExpressionL(type, span, index);
                break;

            case >= '0' and <= '9':
                StoreExpressionNumeric(type, span, index);
                break;
            default:
                throw new FormatException($"Unexpected character: {span[index]}");
        }
    }

    static void CheckIncrementRange(int increment, int type)
    {
        switch (type)
        {
            case CronExpressionConstants.Second or CronExpressionConstants.Minute when increment > 59:
                throw new FormatException($"Increment > 59 : {increment}");
            case CronExpressionConstants.Hour when increment > 23:
                throw new FormatException($"Increment > 23 : {increment}");
            case CronExpressionConstants.DayOfMonth when increment > 31:
                throw new FormatException($"Increment > 31 : {increment}");
            case CronExpressionConstants.DayOfWeek when increment > 7:
                throw new FormatException($"Increment > 7 : {increment}");
            case CronExpressionConstants.Month when increment > 12:
                throw new FormatException($"Increment > 12 : {increment}");
        }
    }

    void CheckNext(int position, ReadOnlySpan<char> span, int value, int type)
    {
        if (position >= span.Length)
        {
            AddToSet(value, -1, -1, type);
            return;
        }

        switch (span[position])
        {
            case 'L':
                HandleLOption(value, type, position);
                return;

            case 'W':
                HandleWOption(value, type, position);
                return;

            case '#':
                HandleHashOption(span, value, type, position);
                return;

            case 'C':
                HandleCOption(value, type, position);
                return;

            case '-':
                HandleDashOption(span, value, type, position);
                return;

            case '/':
                HandleSlashOption(span, value, type, position, -1);
                return;

            default:
                AddToSet(value, -1, 0, type);
                return;
        }
    }

    void HandleSlashOption(ReadOnlySpan<char> span, int value, int type, int index, int end)
    {
        if (index + 1 >= span.Length || char.IsWhiteSpace(span[index + 1]))
            throw new FormatException("\'/\' must be followed by an integer.");

        index++;
        var ch = span[index];
        var charValue = ToInt32(ch);
        index++;
        if (index >= span.Length)
        {
            CheckIncrementRange(charValue, type);
            AddToSet(value, end, charValue, type);
            return;
        }

        ch = span[index];
        if (char.IsDigit(ch))
        {
            var (nextValue, _) = GetValue(charValue, span, index);
            CheckIncrementRange(nextValue, type);
            AddToSet(value, end, nextValue, type);
            return;
        }

        throw new FormatException($"Unexpected character '{ch}' after '/'");
    }

    void HandleDashOption(ReadOnlySpan<char> span, int value, int type, int index)
    {
        index++;
        var ch = span[index];
        var charValue = ToInt32(ch);
        var end = charValue;
        index++;
        if (index >= span.Length)
        {
            AddToSet(value, end, 1, type);
            return;
        }

        ch = span[index];
        if (char.IsDigit(ch))
            (end, index) = GetValue(charValue, span, index);

        if (index < span.Length && span[index] == '/')
        {
            index++;
            ch = span[index];
            var endValue = ToInt32(ch);
            index++;
            if (index >= span.Length)
            {
                AddToSet(value, end, endValue, type);
                return;
            }

            ch = span[index];
            if (char.IsDigit(ch))
            {
                var (nextEndValue, _) = GetValue(endValue, span, index);
                AddToSet(value, end, nextEndValue, type);
                return;
            }

            AddToSet(value, end, endValue, type);
            return;
        }

        AddToSet(value, end, 1, type);
    }

    void HandleCOption(int value, int type, int index)
    {
        switch (type)
        {
            case CronExpressionConstants.DayOfWeek:
                _calendarDayOfWeek = true;
                break;
            case CronExpressionConstants.DayOfMonth:
                _calendarDayOfMonth = true;
                break;
            default:
                throw new FormatException($"'C' option is not valid here. (pos={index})");
        }

        var data = GetSet(type);
        data.Add(value);
    }

    // ReSharper disable once RedundantAssignment
    void HandleHashOption(ReadOnlySpan<char> span, int value, int type, int index)
    {
        var pos = index;
        if (type != CronExpressionConstants.DayOfWeek)
            throw new FormatException($"'#' option is not valid here. (pos={index})");

        index++;
        try
        {
            _nthDayOfWeek = ToInt32(span.Slice(index));
            if (_nthDayOfWeek is < 1 or > 5)
                throw new FormatException("nthDayOfWeek is < 1 or > 5");

        #if NET6_0_OR_GREATER
            if (int.TryParse(span.Slice(0, pos), out value))
        #else
            if (int.TryParse(span.Slice(0, pos).ToString(), out value))
        #endif
            {
                if (value is < 1 or > 7)
                    throw new FormatException("Day-of-Week values must be between 1 and 7");
            }
        }
        catch (Exception)
        {
            throw new FormatException("A numeric value between 1 and 5 must follow the '#' option");
        }

        var set = GetSet(type);
        set.Add(value);
    }

    void HandleWOption(int value, int type, int index)
    {
        if (type == CronExpressionConstants.DayOfMonth)
            _nearestWeekday = true;
        else
            throw new FormatException($"'W' option is not valid here. (pos={index})");

        if (value > 31)
            throw new FormatException("The 'W' option does not make sense with values larger than 31 (max number of days in a month)");

        var data = GetSet(type);
        data.Add(value);
    }

    void HandleLOption(int value, int type, int position)
    {
        if (type == CronExpressionConstants.DayOfWeek)
        {
            if (value is < 1 or > 7)
                throw new FormatException("Day-of-Week values must be between 1 and 7");

            _lastDayOfWeek = true;
        }
        else
            throw new FormatException($"'L' option is not valid here. (pos={position})");

        var data = GetSet(type);
        data.Add(value);
    }

    public string GetExpressionSummary()
    {
        return new CronExpressionSummary(
            _seconds,
            _minutes,
            _hours,
            _daysOfMonth,
            _months,
            _daysOfWeek,
            _lastDayOfWeek,
            _nearestWeekday,
            _nthDayOfWeek,
            _lastDayOfMonth,
            _calendarDayOfWeek,
            _calendarDayOfMonth,
            _years
        ).ToString();
    }

    static int SkipWhiteSpace(int position, ReadOnlySpan<char> span)
    {
        for (; position < span.Length && char.IsWhiteSpace(span[position]); position++)
        {
        }

        return position;
    }

    static int FindNextWhiteSpace(int position, ReadOnlySpan<char> span)
    {
        for (; position < span.Length && !char.IsWhiteSpace(span[position]); position++)
        {
        }

        return position;
    }

    static (int min, int max, string errorMessage) GetValidationParameters(int type)
    {
        return type switch
        {
            CronExpressionConstants.Second or CronExpressionConstants.Minute
                => (0, 59, "Minute and Second values must be between 0 and 59"),
            CronExpressionConstants.Hour
                => (0, 23, "Hour values must be between 0 and 23"),
            CronExpressionConstants.DayOfMonth
                => (1, 31, "Day of month values must be between 1 and 31"),
            CronExpressionConstants.Month
                => (1, 12, "Month values must be between 1 and 12"),
            CronExpressionConstants.DayOfWeek
                => (1, 7, "Day-of-Week values must be between 1 and 7"),
            CronExpressionConstants.Year
                => (Defaults.FirstYear, Defaults.LastYear, $"Year values must be between {Defaults.FirstYear} and {Defaults.LastYear}"),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid cron expression type")
        };
    }

    static bool IsSpecialValue(int value, int type)
    {
        return value == CronExpressionConstants.AllSpec ||
            (type is CronExpressionConstants.DayOfMonth or CronExpressionConstants.DayOfWeek && value == CronExpressionConstants.NoSpec);
    }

    static void ValidateSetValues(int value, int end, int type)
    {
        var (min, max, errorMessage) = GetValidationParameters(type);

        if ((value < min || value > max || end > max) && !IsSpecialValue(value, type))
            throw new FormatException(errorMessage);
    }

    static (int startAt, int stopAt) GetRangeForType(int type, int value, int end)
    {
        return type switch
        {
            CronExpressionConstants.Second or CronExpressionConstants.Minute => (GetStartAt(value, 0), GetStopAt(end, 59)),
            CronExpressionConstants.Hour => (GetStartAt(value, 0), GetStopAt(end, 23)),
            CronExpressionConstants.DayOfMonth => (GetStartAt(value, 1), GetStopAt(end, 31)),
            CronExpressionConstants.Month => (GetStartAt(value, 1), GetStopAt(end, 12)),
            CronExpressionConstants.DayOfWeek => (GetStartAt(value, 1), GetStopAt(end, 7)),
            CronExpressionConstants.Year => (GetStartAt(value, Defaults.FirstYear), GetStopAt(end, Defaults.LastYear)),
            _ => throw new ArgumentException("Unexpected type encountered")
        };
    }

    /// <summary>
    /// Gets the max value for the cron expression type.
    /// </summary>
    /// <param name="type"> The type of the cron expression</param>
    /// <param name="startAt"> The start value</param>
    /// <param name="stopAt"> The stop value</param>
    /// <returns>Returns -1 if stopAt is less than startAt otherwise returns the max value for the type</returns>
    static int GetMaxValueForType(int type, int startAt, int stopAt)
    {
        if (stopAt >= startAt)
            return -1;

        return type switch
        {
            CronExpressionConstants.Second or CronExpressionConstants.Minute => 60,
            CronExpressionConstants.Hour => 24,
            CronExpressionConstants.Month => 12,
            CronExpressionConstants.DayOfWeek => 7,
            CronExpressionConstants.DayOfMonth => 31,
            CronExpressionConstants.Year => throw new ArgumentException("Start year must be less than stop year"),
            _ => throw new ArgumentException("Unexpected type encountered")
        };
    }

    static int GetStartAt(int value, int defaultValue)
    {
        return value is -1 or CronExpressionConstants.AllSpec ? defaultValue : value;
    }

    static int GetStopAt(int end, int defaultValue)
    {
        return end == -1 ? defaultValue : end;
    }

    void AddToSet(int value, int end, int increment, int type)
    {
        ValidateSetValues(value, end, type);

        var data = GetSet(type);

        if (increment is 0 or -1 && value != CronExpressionConstants.AllSpec)
        {
            data.Add(value != -1 ? value : CronExpressionConstants.NoSpec);
            return;
        }

        if (value == CronExpressionConstants.AllSpec && increment <= 0)
        {
            data.Add(CronExpressionConstants.AllSpec);
            return;
        }

        var (startAt, stopAt) = GetRangeForType(type, value, end);

        var max = GetMaxValueForType(type, startAt, stopAt);
        if (max != -1)
            stopAt += max;

        for (var i = startAt; i <= stopAt; i += increment)
        {
            if (max == -1)
                data.Add(i);
            else
            {
                var i2 = i % max;

                if (i2 == 0 && type is CronExpressionConstants.Month or CronExpressionConstants.DayOfWeek or CronExpressionConstants.DayOfMonth)
                    i2 = max;

                data.Add(i2);
            }
        }
    }

    public CronField GetSet(int type)
    {
        var field = type switch
        {
            CronExpressionConstants.Second => _seconds,
            CronExpressionConstants.Minute => _minutes,
            CronExpressionConstants.Hour => _hours,
            CronExpressionConstants.DayOfMonth => _daysOfMonth,
            CronExpressionConstants.Month => _months,
            CronExpressionConstants.DayOfWeek => _daysOfWeek,
            CronExpressionConstants.Year => _years,
            _ => default
        };

        if (field is null)
            throw new ArgumentOutOfRangeException(nameof(type));

        return field;
    }

    static ValueAndPosition GetValue(int value, ReadOnlySpan<char> span, int index)
    {
        var ch = span[index];

        var builder = new StringBuilder(span.Length);
        builder.Append(value);

        while (char.IsDigit(ch))
        {
            builder.Append(ch);
            index++;
            if (index >= span.Length)
                break;

            ch = span[index];
        }

        return new ValueAndPosition(Convert.ToInt32(builder.ToString(), CultureInfo.InvariantCulture), index < span.Length ? index : index + 1);
    }

    /// <summary>
    /// Gets the numeric value from string.
    /// </summary>
    static int GetNumericValue(ReadOnlySpan<char> span, int index)
    {
        var end = FindNextWhiteSpace(index, span);

        return ToInt32(span.Slice(index, end - index));
    }

    /// <summary>
    /// Gets the month number.
    /// </summary>
    /// <param name="span">The string to map with.</param>
    /// <returns></returns>
    static int GetMonthNumber(ReadOnlySpan<char> span)
    {
        return span switch
        {
            "JAN" => 0,
            "FEB" => 1,
            "MAR" => 2,
            "APR" => 3,
            "MAY" => 4,
            "JUN" => 5,
            "JUL" => 6,
            "AUG" => 7,
            "SEP" => 8,
            "OCT" => 9,
            "NOV" => 10,
            "DEC" => 11,
            _ => -1
        };
    }

    static int GetDayOfWeekNumber(ReadOnlySpan<char> span)
    {
        return span switch
        {
            "SUN" => 1,
            "MON" => 2,
            "TUE" => 3,
            "WED" => 4,
            "THU" => 5,
            "FRI" => 6,
            "SAT" => 7,
            _ => -1
        };
    }

    /// <summary>
    /// Progress next fire time seconds
    /// </summary>
    NextFireTimeCursor ProgressNextFireTimeSecond(DateTimeOffset date)
    {
        var second = date.Second;
        if (_seconds.TryGetMinValueStartingFrom(second, out var min))
            second = min;
        else
        {
            second = _seconds.Min;
            date = date.AddMinutes(1);
        }

        return new NextFireTimeCursor(false,
            new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, date.Minute, second, date.Millisecond, date.Offset));
    }

    /// <summary>
    /// Progress next Fire time Minutes
    /// </summary>
    /// <param name="date">NextFireTimeCheck</param>
    NextFireTimeCursor ProgressNextFireTimeMinute(DateTimeOffset date)
    {
        var minute = date.Minute;
        var hour = date.Hour;
        var t = -1;

        if (_minutes.TryGetMinValueStartingFrom(minute, out var min))
        {
            t = minute;
            minute = min;
        }
        else
        {
            minute = _minutes.Min;
            hour++;
        }

        if (minute != t)
        {
            date = new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, minute, 0, date.Millisecond, date.Offset);
            date = SetCalendarHour(date, hour);
            return new NextFireTimeCursor(true, date);
        }

        return new NextFireTimeCursor(false,
            new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, minute, date.Second, date.Millisecond, date.Offset));
    }

    /// <summary>
    /// Progress next fire time Hour
    /// </summary>
    /// <param name="date">NextFireTimeCheck</param>
    NextFireTimeCursor ProgressNextFireTimeHour(DateTimeOffset date)
    {
        int hour;
        var day = date.Day;
        var t = -1;

        if (_hours.TryGetMinValueStartingFrom(date.Hour, out var min))
        {
            t = date.Hour;
            hour = min;
        }
        else
        {
            hour = _hours.Min;
            day++;
        }

        if (hour != t)
        {
            var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            date = day > daysInMonth
                ? new DateTimeOffset(date.Year, date.Month, daysInMonth, date.Hour, 0, 0, date.Millisecond, date.Offset).AddDays(day - daysInMonth)
                : new DateTimeOffset(date.Year, date.Month, day, date.Hour, 0, 0, date.Millisecond, date.Offset);

            date = SetCalendarHour(date, hour);
            return new NextFireTimeCursor(true, date);
        }

        return new NextFireTimeCursor(false,
            new DateTimeOffset(date.Year, date.Month, date.Day, hour, date.Minute, date.Second, date.Millisecond, date.Offset));
    }

    (SortedSet<int> daysOfMonthSet, bool dayHasNegativeOffset) CalculateDaysOfMonth(DateTimeOffset date)
    {
        var daysOfMonthSet = new SortedSet<int>(_daysOfMonth);
        var dayHasNegativeOffset = false;

        if (_lastDayOfMonth)
        {
            var lastDayOfMonthValue = GetLastDayOfMonth(date.Month, date.Year);
            var lastDayOfMonthWithOffset = lastDayOfMonthValue - _lastDayOffset;

            if (_nearestWeekday)
            {
                var calculatedLastDay = CalculateNearestWeekdayForLastDay(date, lastDayOfMonthWithOffset);
                daysOfMonthSet.Add(calculatedLastDay);
            }
            else
                daysOfMonthSet.Add(lastDayOfMonthWithOffset);
        }
        else if (_nearestWeekday)
            (daysOfMonthSet, dayHasNegativeOffset) = CalculateNearestWeekdayForDaysOfMonth(date, daysOfMonthSet);

        return (daysOfMonthSet, dayHasNegativeOffset);
    }

    int CalculateNearestWeekdayForLastDay(DateTimeOffset date, int lastDayOfMonthWithOffset)
    {
        var checkDay = new DateTimeOffset(date.Year, date.Month, lastDayOfMonthWithOffset, date.Hour, date.Minute, date.Second, date.Millisecond, date.Offset);
        var calculatedDay = lastDayOfMonthWithOffset;

        switch (checkDay.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                calculatedDay -= 1;
                break;
            case DayOfWeek.Sunday:
                calculatedDay -= 2;
                break;
        }

        var calculatedLastDayWithOffset = calculatedDay - _lastWeekdayOffset;

        if (calculatedLastDayWithOffset <= 0)
            calculatedLastDayWithOffset = 1;

        return calculatedLastDayWithOffset;
    }

    static (SortedSet<int> daysOfMonthSet, bool dayHasNegativeOffset) CalculateNearestWeekdayForDaysOfMonth(DateTimeOffset date, SortedSet<int> daysOfMonthSet)
    {
        var endDayOfMonth = GetLastDayOfMonth(date.Month, date.Year);
        var minDay = daysOfMonthSet.Min > endDayOfMonth ? endDayOfMonth : daysOfMonthSet.Min;

        var firstDayOfMonth = new DateTimeOffset(date.Year, date.Month, minDay, 0, 0, 0, date.Offset);
        var dayOfWeek = firstDayOfMonth.DayOfWeek;

        if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            daysOfMonthSet.Remove(minDay);

        var (adjustedDay, dayHasNegativeOffset) = AdjustDayToNearestWeekday(minDay, dayOfWeek, endDayOfMonth);
        daysOfMonthSet.Add(adjustedDay);

        return (daysOfMonthSet, dayHasNegativeOffset);
    }

    static (int day, bool dayHasNegativeOffset) AdjustDayToNearestWeekday(int day, DayOfWeek dayOfWeek, int endDayOfMonth)
    {
        var dayHasNegativeOffset = false;

        switch (dayOfWeek)
        {
            case DayOfWeek.Saturday when day == 1:
                day += 2;
                break;
            case DayOfWeek.Saturday:
                day -= 1;
                dayHasNegativeOffset = true;
                break;
            case DayOfWeek.Sunday when day == endDayOfMonth:
                day -= 2;
                dayHasNegativeOffset = true;
                break;
            case DayOfWeek.Sunday:
                day += 1;
                break;
        }

        return (day, dayHasNegativeOffset);
    }

    NextFireTimeCursor ProgressNextFireTimeDayOfMonth(DateTimeOffset date)
    {
        var day = date.Day;
        var month = date.Month;
        var tDay = -1;
        var tMonth = month;

        // get day by day of month rule
        (SortedSet<int>? daysOfMonthCalculated, var setIncludesDayBeforeStartDay) = CalculateDaysOfMonth(date);
        if (daysOfMonthCalculated.TryGetMinValueStartingFrom(date, setIncludesDayBeforeStartDay, out var min))
        {
            tDay = day;
            day = min;

            // make sure we don't over-run a short month, such as february
            var lastDay = GetLastDayOfMonth(month, date.Year);
            if (day > lastDay)
            {
                day = daysOfMonthCalculated.Min;
                month++;
            }
        }
        else
        {
            day = _lastDayOfMonth ? daysOfMonthCalculated.Min : _daysOfMonth.Min;

            month++;
        }

        if (day != tDay || month != tMonth)
        {
            if (month > 12)
                date = new DateTimeOffset(date.Year, 12, day, 0, 0, 0, date.Offset).AddMonths(month - 12);
            else
            {
                var daysInMonth = DateTime.DaysInMonth(date.Year, month);

                date = day <= daysInMonth
                    ? new DateTimeOffset(date.Year, month, day, 0, 0, 0, date.Offset)
                    : new DateTimeOffset(date.Year, month, daysInMonth, 0, 0, 0, date.Offset).AddDays(day - daysInMonth);
            }

            return new NextFireTimeCursor(true, date);
        }

        return new NextFireTimeCursor(false, date);
    }

    NextFireTimeCursor ProgressNextFireTimeDayOfWeek(DateTimeOffset date)
    {
        var day = date.Day;
        var month = date.Month;

        if (_lastDayOfWeek)
        {
            var dayOfWeek = _daysOfWeek.Min;
            var currentDayOfWeek = (int)date.DayOfWeek + 1;
            var daysToAdd = 0;
            if (currentDayOfWeek < dayOfWeek)
                daysToAdd = dayOfWeek - currentDayOfWeek;

            if (currentDayOfWeek > dayOfWeek)
                daysToAdd = dayOfWeek + (7 - currentDayOfWeek);

            var lastDayOfMonth = GetLastDayOfMonth(month, date.Year);

            if (day + daysToAdd > lastDayOfMonth)
            {
                if (month == 12)
                    date = new DateTimeOffset(date.Year, month - 11, 1, 0, 0, 0, date.Offset).AddYears(1);
                else
                    date = new DateTimeOffset(date.Year, month + 1, 1, 0, 0, 0, date.Offset);

                return new NextFireTimeCursor(true, date);
            }

            while (day + daysToAdd + 7 <= lastDayOfMonth)
                daysToAdd += 7;

            day += daysToAdd;

            if (daysToAdd > 0)
                return new NextFireTimeCursor(true, new DateTimeOffset(date.Year, month, day, 0, 0, 0, date.Offset));
        }
        else if (_nthDayOfWeek != 0)
        {
            var dayOfWeek = _daysOfWeek.Min;
            var currentDayOfWeek = (int)date.DayOfWeek + 1;
            var daysToAdd = 0;
            if (currentDayOfWeek < dayOfWeek)
                daysToAdd = dayOfWeek - currentDayOfWeek;
            else if (currentDayOfWeek > dayOfWeek)
                daysToAdd = dayOfWeek + (7 - currentDayOfWeek);

            var dayShifted = daysToAdd > 0;

            day += daysToAdd;
            var weekOfMonth = day / 7;
            if (day % 7 > 0)
                weekOfMonth++;

            daysToAdd = (_nthDayOfWeek - weekOfMonth) * 7;
            day += daysToAdd;
            if (daysToAdd < 0 || day > GetLastDayOfMonth(month, date.Year))
            {
                date = month == 12
                    ? new DateTimeOffset(date.Year, month - 11, 1, 0, 0, 0, date.Offset).AddYears(1)
                    : new DateTimeOffset(date.Year, month + 1, 1, 0, 0, 0, date.Offset);

                return new NextFireTimeCursor(true, date);
            }

            if (daysToAdd > 0 || dayShifted)
                return new NextFireTimeCursor(true, new DateTimeOffset(date.Year, month, day, 0, 0, 0, date.Offset));
        }
        else if (_everyNthWeek != 0)
        {
            var currentDayOfWeek = (int)date.DayOfWeek + 1;
            var dayOfWeek = _daysOfWeek.Min;
            if (_daysOfWeek.TryGetMinValueStartingFrom(currentDayOfWeek, out var min))
                dayOfWeek = min;

            var daysToAdd = 0;
            if (currentDayOfWeek < dayOfWeek)
                daysToAdd = dayOfWeek - currentDayOfWeek + 7 * (_everyNthWeek - 1);

            if (currentDayOfWeek > dayOfWeek)
                daysToAdd = dayOfWeek + (7 - currentDayOfWeek) + 7 * (_everyNthWeek - 1);

            if (daysToAdd > 0)
            {
                date = new DateTimeOffset(date.Year, month, day, 0, 0, 0, date.Offset);
                date = date.AddDays(daysToAdd);
                return new NextFireTimeCursor(true, date);
            }
        }
        else
        {
            var currentDayOfWeek = (int)date.DayOfWeek + 1;
            var dayOfWeek = _daysOfWeek.Min;
            if (_daysOfWeek.TryGetMinValueStartingFrom(currentDayOfWeek, out var min))
                dayOfWeek = min;

            var daysToAdd = 0;
            if (currentDayOfWeek < dayOfWeek)
                daysToAdd = dayOfWeek - currentDayOfWeek;

            if (currentDayOfWeek > dayOfWeek)
                daysToAdd = dayOfWeek + (7 - currentDayOfWeek);

            var lDay = GetLastDayOfMonth(month, date.Year);

            if (day + daysToAdd > lDay)
            {
                date = month == 12
                    ? new DateTimeOffset(date.Year, month - 11, 1, 0, 0, 0, date.Offset).AddYears(1)
                    : new DateTimeOffset(date.Year, month + 1, 1, 0, 0, 0, date.Offset);

                return new NextFireTimeCursor(true, date);
            }

            if (daysToAdd > 0)
                return new NextFireTimeCursor(true, new DateTimeOffset(date.Year, month, day + daysToAdd, 0, 0, 0, date.Offset));
        }

        return new NextFireTimeCursor(false, new DateTimeOffset(date.Year, date.Month, day, date.Hour, date.Minute, date.Second, date.Offset));
    }

    NextFireTimeCursor ProgressNextFireTimeDay(DateTimeOffset date)
    {
        var dayOfMonthSpec = !_daysOfMonth.Contains(CronExpressionConstants.NoSpec);
        var dayOfWeekSpec = !_daysOfWeek.Contains(CronExpressionConstants.NoSpec);
        if (dayOfMonthSpec && !dayOfWeekSpec)
            return ProgressNextFireTimeDayOfMonth(date);

        if (dayOfWeekSpec && !dayOfMonthSpec)
            return ProgressNextFireTimeDayOfWeek(date);

        var dayOfMonthProgressResult = ProgressNextFireTimeDayOfMonth(date);
        var dayOfWeekProgressResult = ProgressNextFireTimeDayOfWeek(date);
        if (dayOfMonthProgressResult.RestartLoop && dayOfWeekProgressResult.RestartLoop)
        {
            return dayOfWeekProgressResult.Date < dayOfMonthProgressResult.Date
                ? dayOfWeekProgressResult
                : dayOfMonthProgressResult;
        }

        if (dayOfWeekProgressResult is { Date: not null, RestartLoop: false })
            return dayOfWeekProgressResult;
        if (dayOfMonthProgressResult is { Date: not null, RestartLoop: false })
            return dayOfMonthProgressResult;

        return dayOfWeekProgressResult.Date!.Value < dayOfMonthProgressResult.Date!.Value
            ? dayOfWeekProgressResult
            : dayOfMonthProgressResult;
    }

    NextFireTimeCursor ProgressNextFireTimeMonth(DateTimeOffset date)
    {
        var month = date.Month;
        var year = date.Year;
        var tMonth = -1;

        if (_months.TryGetMinValueStartingFrom(month, out var min))
        {
            tMonth = month;
            month = min;
        }
        else
        {
            month = _months.Min;
            year++;
        }

        return month != tMonth
            ? new NextFireTimeCursor(true, new DateTimeOffset(year, month, 1, 0, 0, 0, date.Offset))
            : new NextFireTimeCursor(false, new DateTimeOffset(date.Year, month, date.Day, date.Hour, date.Minute, date.Second, date.Offset));
    }

    NextFireTimeCursor ProgressNextFireTimeYear(DateTimeOffset date)
    {
        var year = date.Year;
        int tYear;
        if (_years.TryGetMinValueStartingFrom(date.Year, out var min))
        {
            tYear = year;
            year = min;
        }
        else
            return new NextFireTimeCursor(false, null);

        return year != tYear
            ? new NextFireTimeCursor(true, new DateTimeOffset(year, 1, 1, 0, 0, 0, date.Offset))
            : new NextFireTimeCursor(false, new DateTimeOffset(year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Offset));
    }

    public DateTimeOffset? GetTimeAfter(DateTimeOffset afterTime)
    {
        afterTime = afterTime.AddSeconds(1);

        var date = StripMilliseconds(afterTime);

        date = TimeZoneUtil.ConvertTime(date, TimeZone);

        Func<DateTimeOffset, NextFireTimeCursor>[] nextFireTimeProgressions =
        {
            ProgressNextFireTimeSecond,
            ProgressNextFireTimeMinute,
            ProgressNextFireTimeHour,
            ProgressNextFireTimeDay,
            ProgressNextFireTimeMonth,
            ProgressNextFireTimeYear
        };

        var nextFireTimeCursor = new NextFireTimeCursor(false, date);
        var foundNextFireTime = false;

        while (!foundNextFireTime)
        {
            foreach (Func<DateTimeOffset, NextFireTimeCursor> progression in nextFireTimeProgressions)
            {
                if (nextFireTimeCursor.Date.HasValue)
                    nextFireTimeCursor = progression(nextFireTimeCursor.Date.Value);
                else
                    break;

                if (nextFireTimeCursor.RestartLoop)
                    break;
            }

            if (nextFireTimeCursor.Date is null || nextFireTimeCursor.Date.Value.Year > Defaults.LastYear)
                return null;

            if (nextFireTimeCursor.RestartLoop)
                continue;

            date = new DateTimeOffset(nextFireTimeCursor.Date.Value.DateTime, TimeZoneUtil.GetUtcOffset(nextFireTimeCursor.Date.Value.DateTime, TimeZone));
            foundNextFireTime = true;
        }

        return date.ToUniversalTime();
    }

    static DateTimeOffset StripMilliseconds(DateTimeOffset time)
    {
        return new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Offset);
    }

    static DateTimeOffset SetCalendarHour(DateTimeOffset date, int hour)
    {
        var hourToSet = hour;
        if (hourToSet == 24)
            hourToSet = 0;

        var d = new DateTimeOffset(date.Year, date.Month, date.Day, hourToSet, date.Minute, date.Second, date.Millisecond, date.Offset);
        if (hour == 24)
            d = d.AddDays(1);

        return d;
    }

    static int GetLastDayOfMonth(int month, int year)
    {
        return DateTime.DaysInMonth(year, month);
    }

    static int ToInt32(char c)
    {
        return c - '0';
    }

    static int ToInt32(ReadOnlySpan<char> span)
    {
    #if NET6_0_OR_GREATER
        return int.Parse(span);
    #else
        return int.Parse(span.ToString());
    #endif
    }
}
