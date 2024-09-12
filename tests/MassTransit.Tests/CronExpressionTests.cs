namespace MassTransit.Tests;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JobService.Scheduling;
using NUnit.Framework;


public class CronExpressionTest
{
    static readonly TimeZoneInfo TestTimeZone = TimeZoneInfo.Local;

    /// <summary>
    /// Test method for 'CronExpression.IsSatisfiedBy(DateTime)'.
    /// </summary>
    [Test]
    public void TestIsSatisfiedBy()
    {
        var cronExpression = new CronExpression("0 15 10 * * ? 2005");

        var cal = new DateTime(2005, 6, 1, 10, 15, 0).ToUniversalTime();
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.True);

        cal = cal.AddYears(1);
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.False);

        cal = new DateTime(2005, 6, 1, 10, 16, 0).ToUniversalTime();
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.False);

        cal = new DateTime(2005, 6, 1, 10, 14, 0).ToUniversalTime();
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.False);

        cronExpression = new CronExpression("0 15 10 ? * MON-FRI");

        // weekends
        cal = new DateTime(2007, 6, 9, 10, 15, 0).ToUniversalTime();
        Assert.Multiple(() =>
        {
            Assert.That(cronExpression.IsSatisfiedBy(cal), Is.False);
            Assert.That(cronExpression.IsSatisfiedBy(cal.AddDays(1)), Is.False);
        });
    }

    [Test]
    public void TestLastDayOffset()
    {
        var cronExpression = new CronExpression("0 15 10 L-2 * ? 2010");

        var cal = new DateTime(2010, 10, 29, 10, 15, 0).ToUniversalTime(); // last day - 2
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.True);

        cal = new DateTime(2010, 10, 28, 10, 15, 0).ToUniversalTime();
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.False);

        cronExpression = new CronExpression("0 15 10 L-5W * ? 2010");

        cal = new DateTime(2010, 10, 26, 10, 15, 0).ToUniversalTime(); // last day - 5
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.True);

        cronExpression = new CronExpression("0 15 10 L-1 * ? 2010");

        cal = new DateTime(2010, 10, 30, 10, 15, 0).ToUniversalTime(); // last day - 1
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.True);

        cronExpression = new CronExpression("0 15 10 L-1W * ? 2010");

        cal = new DateTime(2010, 10, 29, 10, 15, 0).ToUniversalTime(); // nearest weekday to last day - 1 (29th is a friday in 2010)
        Assert.That(cronExpression.IsSatisfiedBy(cal), Is.True);
    }

    [TestCase("0 15 10 6,15 * ? 2010", "0 15 10 6,15 * ? 2010")]
    public void ExpressionToString(string cronExpression, string expected)
    {
        var expr = new CronExpression(cronExpression);
        Assert.That(expr.ToString(), Is.EqualTo(expected));
    }

    [TestCase("0 15 10 L-1,L-2 * ? 2010", new[] { 31 - 1, 31 - 2 })] //Multiple L Not supported
    public void CannotUseMultipleLastDayOfMonthInArray(string cronExpression, int[] expectedDays, string scenario = "")
    {
        Action act = () => new CronExpression(cronExpression); //10:15am <variable days> October 2010
        Assert.That(() => act(), Throws.InstanceOf<FormatException>().With.Message.EqualTo(
            "Support for specifying 'L' with other days of the month is limited to one instance of L"));
    }

    [TestCase("0 15 10 6,15,LW * ? 2010", new[] { 6, 15, 29 })] //31 oct 2010 is a Sunday, week day would be 29
    [TestCase("0 15 10 6,15,L * ? 2010", new[] { 6, 15, 31 })]
    [TestCase("0 15 10 15,L * ? 2010", new[] { 15, 31 })]
    [TestCase("0 15 10 15,31 * ? 2010", new[] { 15, 31 })]
    [TestCase("0 15 10 15,L-2 * ? 2010", new[] { 15, 31 - 2 })]
    [TestCase("0 15 10 31,L-2 * ? 2010", new[] { 31 }, "duplicate day specified + last are equal")]
    [TestCase("0 15 10 1,3,6,15,L * ? 2010", new[] { 1, 3, 6, 15, 31 })]
    [TestCase("0 15 10 15,LW-2 * ? 2010", new[] { 15, 29 - 2 })] //29 is last week day
    public void CanUseLastDayOfMonthInArray(string cronExpression, int[] expectedDays, string scenario = "")
    {
        var expr = new CronExpression(cronExpression); //10:15am <variable days> October 2010

        foreach (var expectedDay in expectedDays)
        {
            var date = new DateTime(2010, 10, expectedDay, 10, 15, 0).ToUniversalTime(); // last day
            Assert.That(expr.IsSatisfiedBy(date), Is.True, $"expected day of {expectedDay}, {scenario}");
        }
    }

    int[] CreateArrayOfDays(int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var numbers = new List<int>();
        for (var i = 0; i < daysInMonth; i++)
            numbers.Add(i + 1);

        return numbers.ToArray();
    }

    [TestCase("0 15 10 5/5 * MON 2010", new[] { 4, 11, 18, 25, 5, 10, 15, 20, 25, 30 },
        "10:15am every 5th day of the month from 5 to 31, and on Mondays in October 2010")]
    [TestCase("0 15 10 3 * MON,THU,FRI 2010", new[] { 1, 3, 4, 11, 18, 25, 7, 14, 21, 28, 8, 15, 22, 29 },
        "10:15am 3rd of month and every mon,thu,fri October 2010")]
    [TestCase("0 15 10 1,2,3,4,5,6 * MON,THU,FRI 2010", new[] { 1, 2, 3, 4, 5, 6, 11, 18, 25, 7, 14, 21, 28, 8, 15, 22, 29 },
        "10:15am 1-6th of mon and every Mon,Thu,Fri October 2010")]
    [TestCase("0 15 10 * * MON,THU,FRI 2010",
        new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 },
        "10:15am EveryDay of Month October 2010, Wildcard specified")]
    [TestCase("0 15 10 1 * * 2010", new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 },
        "10:15am Every Day of Month October 2010, Wildcard specified")]
    public void CanUse_DayOfMonth_And_DayOfWeek_Together(string cronExpression, int[] expectedDays, string scenario = "")
    {
        var expr = new CronExpression(cronExpression);
        var templateDate = new DateTime(2010, 10, 1, 10, 15, 0).ToUniversalTime();

        foreach (var day in expectedDays)
        {
            var date = new DateTime(templateDate.Year, templateDate.Month, day, templateDate.Hour, templateDate.Minute, templateDate.Second, templateDate.Kind);
            Assert.That(expr.IsSatisfiedBy(date), Is.True, $"expected day of {day}, {scenario}");
        }

        IEnumerable<int> invalidDays = CreateArrayOfDays(2010, 10).Except(expectedDays);

        foreach (var day in invalidDays)
        {
            var date = new DateTime(templateDate.Year, templateDate.Month, day, templateDate.Hour, templateDate.Minute, templateDate.Second, templateDate.Kind);
            Assert.That(expr.IsSatisfiedBy(date), Is.False, $"invalid day of {day}, {scenario}");
        }
    }

    [TestCase("0 15 10 LW-2 * ? 2010", 27, "31 Oct 2010 is Sunday, last-weekday (LW) is 29 (FRI) -2 Offset")]
    [TestCase("0 15 10 LW-5 * ? 2010", 24, "31 Oct 2010 is Sunday, last-weekday (LW) is 29 (FRI) -5 Offset")]
    [TestCase("0 15 10 LW-7 * ? 2010", 22, "31 Oct 2010 is Sunday, last-weekday (LW) is 29 (FRI) -7 Offset")]
    [TestCase("0 15 10 LW-28 * ? 2010", 1, "31 Oct 2010 is Sunday, last-weekday (LW) is 29 (FRI) -28 Offset")]
    [TestCase("0 15 10 LW-29 * ? 2010", 1, "31 Oct 2010 is Sunday, last-weekday (LW) is 29 (FRI) -29 Offset fallback to 1st of month")]
    [TestCase("0 15 10 LW-30 * ? 2010", 1, "31 Oct 2010 is Sunday, last-weekday (LW) is 29 (FRI) -30 Offset fallback to 1st of month")]
    public void LastWeekDayWithOffset(string cronExpression, int expectedDay, string reason)
    {
        var expr = new CronExpression(cronExpression);
        var date = new DateTime(2010, 10, expectedDay, 10, 15, 0).ToUniversalTime(); // last day
        Assert.That(expr.IsSatisfiedBy(date), Is.True, reason);
    }

    [TestCase("0 15 10 ? * 1#0 2010", false)]
    [TestCase("0 15 10 ? * 1#1 2010", true)]
    [TestCase("0 15 10 ? * 1#2 2010", true)]
    [TestCase("0 15 10 ? * 1#3 2010", true)]
    [TestCase("0 15 10 ? * 1#4 2010", true)]
    [TestCase("0 15 10 ? * 1#5 2010", true)]
    [TestCase("0 15 10 ? * 1#6 2010", false)]
    public void Ensure_NthWeek_IsBetween1And5(string expression, bool isValid)
    {
        Action act = () => new CronExpression(expression); //10:15am <variable days> October 2010
        if (isValid)
            Assert.That(() => act(), Throws.Nothing);
        else
            Assert.That(() => act(), Throws.InstanceOf<FormatException>());
    }

    [TestCase("0 15 10 ? * 0#1 2010", false)]
    [TestCase("0 15 10 ? * 1#1 2010", true, "2010-01-03T10:15:00")]
    [TestCase("0 15 10 ? * 2#1 2010", true, "2010-01-04T10:15:00")]
    [TestCase("0 15 10 ? * 3#1 2010", true, "2010-01-05T10:15:00")]
    [TestCase("0 15 10 ? * 4#1 2010", true, "2010-01-06T10:15:00")]
    [TestCase("0 15 10 ? * 5#1 2010", true, "2010-01-07T10:15:00")]
    [TestCase("0 15 10 ? * 6#1 2010", true, "2010-01-01T10:15:00")]
    [TestCase("0 15 10 ? * 7#1 2010", true, "2010-01-02T10:15:00")]
    [TestCase("0 15 10 ? * 8#1 2010", false)]
    [TestCase("0 15 10 ? * 14#1 2010", false)]
    public void Ensure_NthWeek_Day_IsBetween1And7(string expression, bool isValid, string shouldSatisfyDate = null)
    {
        Action act = () => new CronExpression(expression);
        if (isValid)
        {
            Assert.That(() => act(), Throws.Nothing);

            var exp = new CronExpression(expression);
            if (!string.IsNullOrEmpty(shouldSatisfyDate))
            {
                var dt = DateTime.Parse(shouldSatisfyDate);
                Assert.That(exp.IsSatisfiedBy(new DateTimeOffset(dt)));
            }
        }
        else
            Assert.That(() => act(), Throws.InstanceOf<FormatException>());
    }

    [TestCase("0 15 10 6,15,LW * ? 2010")]
    [TestCase("0 15 10 6,15,L * ? 2010")]
    [TestCase("0 15 10 15,L * ? 2010")]
    [TestCase("0 15 10 15,31 * ? 2010")]
    [TestCase("0 15 10 15,L-2 * ? 2010")]
    [TestCase("0 15 10 31,L-2 * ? 2010")]
    [TestCase("0 15 10 1,3,6,15,L * ? 2010")]
    public void ExpressionEquality(string expression)
    {
        var expr1 = new CronExpression(expression);
        var expr2 = new CronExpression(expression);
        Assert.That(expr1, Is.EqualTo(expr2));

        Assert.That((object)expr1, Is.EqualTo(expr2));
    }

    [TestCase("0 15 10 15,L-31 * ? 2010")]
    public void OffSetValue_CannontBe_GreaterThan30(string expression)
    {
        Action act = () => new CronExpression(expression);
        Assert.That(() => act(), Throws.InstanceOf<FormatException>().With.Message.EqualTo("Offset from last day must be <= 30"));
    }

    [TestCase("L 15 10 15 * ? 2010", false)]
    [TestCase("0 L 10 15 * ? 2010", false)]
    [TestCase("0 15 L 15 * ? 2010", false)]
    [TestCase("0 15 10 L * ? 2010", true, "Valid for day of month")]
    [TestCase("0 15 10 15 L ? 2010", false)]
    [TestCase("0 15 10 ? * L 2010", true, "Valid for day of week")]
    [TestCase("0 15 10 15 * ? L", false)]
    public void Ensure_L_Token_CanOnlyBeUsedIn_DayOfWeek_ORDayOfMonth(string expression, bool isValid, string description = "")
    {
        Action act = () => new CronExpression(expression);
        if (isValid)
            Assert.That(() => act(), Throws.Nothing, description);
        else
            Assert.That(() => act(), Throws.InstanceOf<FormatException>(), description);
    }

    [Test]
    public void CronExpression_Throw_Error_Constructed_With_Null()
    {
        Action act = () => new CronExpression(null);
        Assert.That(() => act(), Throws.InstanceOf<ArgumentNullException>());
    }

    [TestCase('h')]
    [TestCase('?')]
    [TestCase('*')]
    public void Should_Throw_Error_When_Extra_NonWhitespace_Character_After_QuestionMark(char invalidChar)
    {
        Action act = () => new CronExpression($"0 0 * * * ?{invalidChar}");
        Assert.That(() => act(), Throws.InstanceOf<FormatException>());
    }

    [TestCase(' ')]
    [TestCase('\t')]
    public void QuestionMark_With_ExtraWhitespace_Should_Be_Valid(char allowedChar)
    {
        Action act = () => new CronExpression($"0 0 * * * ?{allowedChar}");
        Assert.That(() => act(), Throws.Nothing);
    }

    [Test]
    public void TestCronExpressionPassingMidnight()
    {
        var cronExpression = new CronExpression("0 15 23 * * ?");
        DateTimeOffset cal = new DateTime(2005, 6, 1, 23, 16, 0).ToUniversalTime();
        DateTimeOffset nextExpectedFireTime = new DateTime(2005, 6, 2, 23, 15, 0).ToUniversalTime();
        Assert.That(cronExpression.GetTimeAfter(cal).Value, Is.EqualTo(nextExpectedFireTime));
    }

    [Test]
    public void TestCronExpressionPassingYear()
    {
        DateTimeOffset start = new DateTime(2007, 12, 1, 23, 59, 59).ToUniversalTime();

        var ce = new CronExpression("0 55 15 1 * ?");
        DateTimeOffset expected = new DateTime(2008, 1, 1, 15, 55, 0).ToUniversalTime();
        var d = ce.GetNextValidTimeAfter(start).Value;
        Assert.That(d, Is.EqualTo(expected), "Got wrong date and time when passed year");
    }

    [Test]
    public void TestCronExpressionWeekdaysMonFri()
    {
        var cronExpression = new CronExpression("0 0 12 ? * MON-FRI");
        int[] arrJuneDaysThatShouldFire =
            [1, 4, 5, 6, 7, 8, 11, 12, 13, 14, 15, 18, 19, 20, 22, 21, 25, 26, 27, 28, 29];
        var juneDays = new List<int>(arrJuneDaysThatShouldFire);

        TestCorrectWeekFireDays(cronExpression, juneDays);
    }

    [Test]
    public void TestCronExpressionWeekdaysFriday()
    {
        var cronExpression = new CronExpression("0 0 12 ? * FRI");
        DateTimeOffset? nextRunTime = cronExpression.GetTimeAfter(DateTimeOffset.Now);
        DateTimeOffset? nextRunTime2 = cronExpression.GetTimeAfter((DateTimeOffset)nextRunTime);

        int[] arrJuneDaysThatShouldFire = { 1, 8, 15, 22, 29 };
        var juneDays = new List<int>(arrJuneDaysThatShouldFire);

        TestCorrectWeekFireDays(cronExpression, juneDays);
    }

    [Test]
    public void TestCronExpressionWeekdaysFridayEveryTwoWeeks()
    {
        var cronExpression = new CronExpression("0 0 12 ? * FRI/2");
        DateTimeOffset? nextRunTime = cronExpression.GetTimeAfter(DateTimeOffset.Now);
        DateTimeOffset? nextRunTime2 = cronExpression.GetTimeAfter((DateTimeOffset)nextRunTime);

        int[] arrJuneDaysThatShouldFire = { 1, 15, 29 };
        var juneDays = new List<int>(arrJuneDaysThatShouldFire);

        TestCorrectWeekFireDays(cronExpression, juneDays);
    }

    [Test]
    public void TestCronExpressionWeekdaysThirsdayAndFridayEveryTwoWeeks()
    {
        var cronExpression = new CronExpression("0 0 12 ? * THU,FRI/2");
        DateTimeOffset? nextRunTime = cronExpression.GetTimeAfter(DateTimeOffset.Now);
        DateTimeOffset? nextRunTime2 = cronExpression.GetTimeAfter((DateTimeOffset)nextRunTime);

        int[] arrJuneDaysThatShouldFire = { 1, 14, 15, 28, 29 };
        var juneDays = new List<int>(arrJuneDaysThatShouldFire);

        TestCorrectWeekFireDays(cronExpression, juneDays);
    }

    [Test]
    public void TestCronExpressionLastDayOfMonth()
    {
        var cronExpression = new CronExpression("0 0 12 L * ?");
        int[] arrJuneDaysThatShouldFire = { 30 };
        var juneDays = new List<int>(arrJuneDaysThatShouldFire);

        TestCorrectWeekFireDays(cronExpression, juneDays);
    }

    [Test]
    public void TestHourShift()
    {
        var cronExpression = new CronExpression("0/5 * * * * ?");
        var cal = new DateTimeOffset(2005, 6, 1, 1, 59, 55, TimeSpan.Zero);
        var nextExpectedFireTime = new DateTimeOffset(2005, 6, 1, 2, 0, 0, TimeSpan.Zero);
        Assert.That(cronExpression.GetTimeAfter(cal).Value, Is.EqualTo(nextExpectedFireTime));
    }

    [Test]
    public void TestEveryMinute()
    {
        var cronExpression = new CronExpression("* * * * * ?");
        var cal = new DateTimeOffset(2005, 6, 1, 1, 59, 55, TimeSpan.Zero);
        var nextExpectedFireTime = new DateTimeOffset(2005, 6, 1, 1, 59, 56, TimeSpan.Zero);
        Assert.That(cronExpression.GetTimeAfter(cal).Value, Is.EqualTo(nextExpectedFireTime));
    }

    [Test]
    public void TestMonthShift()
    {
        var cronExpression = new CronExpression("* * 1 * * ?");
        DateTimeOffset cal = new DateTime(2005, 7, 31, 22, 59, 57).ToUniversalTime();
        DateTimeOffset nextExpectedFireTime = new DateTime(2005, 8, 1, 1, 0, 0).ToUniversalTime();
        Assert.That(cronExpression.GetTimeAfter(cal).Value, Is.EqualTo(nextExpectedFireTime));
    }

    [Test]
    public void TestYearChange()
    {
        var cronExpression = new CronExpression("0 12 4 ? * 3");
        cronExpression.GetNextValidTimeAfter(new DateTime(2007, 12, 28));
    }

    [Test]
    public void TestCronExpressionParsingIncorrectDayOfWeek()
    {
        try
        {
            var expr = $" * * * * * {DateTime.Now.Year}";
            var ce = new CronExpression(expr);
            ce.IsSatisfiedBy(DateTime.UtcNow.AddMinutes(2));
            Assert.Fail("Accepted wrong format");
        }
        catch (FormatException fe)
        {
            Assert.That(fe.Message, Is.EqualTo("Day-of-Week values must be between 1 and 7"));
        }
    }

    [Test]
    public void TestCronExpressionWithExtraWhiteSpace()
    {
        var expr = " 30 *   * * * ?  ";
        var calendar = new CronExpression(expr);
        Assert.That(calendar.IsSatisfiedBy(DateTime.UtcNow.Date.AddMinutes(2)), Is.False, "Time was included");
    }

    static void TestCorrectWeekFireDays(CronExpression cronExpression, IList<int> correctFireDays)
    {
        var fireDays = new List<int>();

        var cal = new DateTime(2007, 6, 1, 11, 0, 0).ToUniversalTime();
        DateTimeOffset? nextFireTime = cal;

        for (var i = 0; i < DateTime.DaysInMonth(2007, 6); ++i)
        {
            nextFireTime = cronExpression.GetTimeAfter((DateTimeOffset)nextFireTime);
            if (!fireDays.Contains(nextFireTime.Value.Day) && nextFireTime.Value.Month == 6 && nextFireTime.Value.Year == 2007)
                fireDays.Add(nextFireTime.Value.Day);
        }

        for (var i = 0; i < fireDays.Count; ++i)
        {
            var idx = correctFireDays.IndexOf(fireDays[i]);
            Assert.That(idx, Is.GreaterThan(-1), $"CronExpression evaluated true for {fireDays[i]} even when it shouldn't have");
            correctFireDays.RemoveAt(idx);
        }

        Assert.That(correctFireDays, Is.Empty, $"CronExpression did not evaluate true for all expected days (count: {correctFireDays.Count}).");
    }

    [Test]
    public void TestNthWeekDayPassingMonth()
    {
        var ce = new CronExpression("0 30 10-13 ? * FRI#3");
        var start = new DateTime(2008, 12, 19, 0, 0, 0);
        for (var i = 0; i < 200; ++i)
        {
            var shouldFire = start.Hour >= 10 && start.Hour <= 13 && start.Minute == 30
                && (start.DayOfWeek == DayOfWeek.Wednesday || start.DayOfWeek == DayOfWeek.Friday);
            shouldFire = shouldFire && start.Day > 15 && start.Day < 28;

            var satisfied = ce.IsSatisfiedBy(start.ToUniversalTime());
            Assert.That(satisfied, Is.EqualTo(shouldFire));

            // cycle with half hour precision
            start = start.AddHours(0.5);
        }
    }

    [Test]
    public void TestNormal()
    {
        for (var i = 0; i < 6; i++)
            AssertParsesForField("0 15 10 * * ? 2005", i);
    }

    [Test]
    public void TestSecond()
    {
        AssertParsesForField("58-4 5 21 ? * MON-FRI", 0);
    }

    [Test]
    public void TestMinute()
    {
        AssertParsesForField("0 58-4 21 ? * MON-FRI", 1);
    }

    [Test]
    public void TestHour()
    {
        AssertParsesForField("0 0/5 21-3 ? * MON-FRI", 2);
    }

    [Test]
    public void TestDayOfWeekNumber()
    {
        AssertParsesForField("58 5 21 ? * 6-2", 5);
    }

    [Test]
    public void TestDayOfWeek()
    {
        AssertParsesForField("58 5 21 ? * FRI-TUE", 5);
    }

    [Test]
    public void TestDayOfMonth()
    {
        AssertParsesForField("58 5 21 28-5 1 ?", 3);
    }

    [Test]
    public void TestMonth()
    {
        AssertParsesForField("58 5 21 ? 11-2 FRI", 4);
    }

    [Test]
    public void TestAmbiguous()
    {
        AssertParsesForField("0 0 14-6 ? * FRI-MON", 2);
        AssertParsesForField("0 0 14-6 ? * FRI-MON", 5);

        AssertParsesForField("55-3 56-2 6 ? * FRI", 0);
        AssertParsesForField("55-3 56-2 6 ? * FRI", 1);
    }

    static void AssertParsesForField(string expression, int constant)
    {
        try
        {
            var cronExpression = new CronExpression(expression);
            var set = cronExpression.GetSet(constant);
            if (set.Count == 0)
                Assert.Fail("Empty field [" + constant + "] returned for " + expression);
        }
        catch (FormatException pe)
        {
            Assert.Fail("Exception thrown during parsing: " + pe);
        }
    }

    [Test]
    public void TestQuartz640()
    {
        try
        {
            new CronExpression("0 43 9 ? * SAT,SUN,L");
            Assert.Fail("Expected FormatException did not fire for L combined with other days of the week");
        }
        catch (FormatException pe)
        {
            Assert.That(
                pe.Message,
                Does.StartWith("Support for specifying 'L' with other days of the week is not implemented"),
                "Incorrect FormatException thrown");
        }

        try
        {
            new CronExpression("0 43 9 ? * 6,7,L");
            Assert.Fail("Expected FormatException did not fire for L combined with other days of the week");
        }
        catch (FormatException pe)
        {
            Assert.That(
                pe.Message,
                Does.StartWith("Support for specifying 'L' with other days of the week is not implemented"),
                "Incorrect FormatException thrown");
        }

        try
        {
            new CronExpression("0 43 9 ? * 5L");
        }
        catch (FormatException)
        {
            Assert.Fail("Unexpected ParseException thrown for supported '5L' expression.");
        }
    }

    [Test]
    public void TestGetTimeAfter_QRTZNET149()
    {
        var expression = new CronExpression("0 0 0 29 * ?");
        DateTimeOffset? after = expression.GetNextValidTimeAfter(new DateTime(2009, 1, 30, 0, 0, 0).ToUniversalTime());
        Assert.Multiple(() =>
        {
            Assert.That(after.HasValue, Is.True);
            Assert.That(after.Value.DateTime, Is.EqualTo(new DateTime(2009, 3, 29, 0, 0, 0).ToUniversalTime()));
        });

        after = expression.GetNextValidTimeAfter(new DateTime(2009, 12, 30).ToUniversalTime());
        Assert.Multiple(() =>
        {
            Assert.That(after.HasValue, Is.True);
            Assert.That(after.Value.DateTime, Is.EqualTo(new DateTime(2010, 1, 29, 0, 0, 0).ToUniversalTime()));
        });
    }

    [Test]
    public void TestQRTZNET152_Nearest_Weekday_Expression_W_Does_Not_Work_In_CronTrigger()
    {
        var expression = new CronExpression("0 5 13 5W 1-12 ?");
        var test = new DateTimeOffset(2009, 3, 8, 0, 0, 0, TimeSpan.Zero); //Sunday
        var d = expression.GetNextValidTimeAfter(test).Value;
        // 2009-04-06 is Monday, Sunday is invalid for W
        Assert.That(d, Is.EqualTo(new DateTimeOffset(2009, 4, 6, 13, 5, 0, TimeZoneUtil.GetUtcOffset(d, TimeZoneInfo.Local)).ToUniversalTime()));
        d = expression.GetNextValidTimeAfter(d).Value;
        Assert.That(d, Is.EqualTo(new DateTimeOffset(2009, 5, 5, 13, 5, 0, TimeZoneUtil.GetUtcOffset(d, TimeZoneInfo.Local))));
    }

    [Test]
    public void ShouldThrowExceptionIfWParameterMakesNoSense()
    {
        try
        {
            new CronExpression("0/5 * * 32W 1 ?");
            Assert.Fail("Expected FormatException did not fire for W with value larger than 31");
        }
        catch (FormatException pe)
        {
            Assert.That(pe.Message, Does.StartWith("The 'W' option does not make sense with values larger than"), "Incorrect ParseException thrown");
        }
    }

    [Test]
    [Platform("WIN")]
    public void TestDaylightSaving_QRTZNETZ186()
    {
        var expression = new CronExpression("0 15 * * * ?");
        if (!TimeZoneInfo.Local.SupportsDaylightSavingTime)
            return;

        var daylightChange = TimeZone.CurrentTimeZone.GetDaylightChanges(2012);
        DateTimeOffset before = daylightChange.Start.ToUniversalTime().AddMinutes(-5); // keep outside the potentially undefined interval
        DateTimeOffset? after = expression.GetNextValidTimeAfter(before);
        Assert.That(after.HasValue, Is.True);
        DateTimeOffset expected = daylightChange.Start.Add(daylightChange.Delta).AddMinutes(15).ToUniversalTime();
        Assert.That(after.Value, Is.EqualTo(expected));
    }

    [Test]
    public void TestDaylightSavingsDoesNotMatchAnHourBefore()
    {
        var est = TimeZoneUtil.FindTimeZoneById("Eastern Standard Time");
        var expression = new CronExpression("0 15 15 5 11 ?");
        expression.TimeZone = est;

        var startTime = new DateTimeOffset(2012, 11, 4, 0, 0, 0, TimeSpan.Zero);

        DateTimeOffset? actualTime = expression.GetTimeAfter(startTime);
        var expected = new DateTimeOffset(2012, 11, 5, 15, 15, 0, TimeSpan.FromHours(-5));

        Assert.That(actualTime.Value, Is.EqualTo(expected));
    }

    [Test]
    public void TestDaylightSavingsDoesNotMatchAnHourBefore2()
    {
        //another case
        var est = TimeZoneUtil.FindTimeZoneById("Eastern Standard Time");
        var expression = new CronExpression("0 0 0 ? * THU");
        expression.TimeZone = est;

        var startTime = new DateTimeOffset(2012, 11, 4, 0, 0, 0, TimeSpan.Zero);

        DateTimeOffset? actualTime = expression.GetTimeAfter(startTime);
        var expected = new DateTimeOffset(2012, 11, 8, 0, 0, 0, TimeSpan.FromHours(-5));
        Assert.That(actualTime, Is.EqualTo(expected));
    }

    [Test]
    public void TestSecRangeIntervalAfterSlash()
    {
        // Test case 1
        var e =
            Assert.Throws<FormatException>(() => new CronExpression("/120 0 8-18 ? * 2-6"), "Cron did not validate bad range interval in '_blank/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 59 : 120"));

        // Test case 2
        e = Assert.Throws<FormatException>(() => new CronExpression("0/120 0 8-18 ? * 2-6"), "Cron did not validate bad range interval in in '0/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 59 : 120"));

        // Test case 3
        e = Assert.Throws<FormatException>(() => new CronExpression("/ 0 8-18 ? * 2-6"), "Cron did not validate bad range interval in '_blank/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));

        // Test case 4
        e = Assert.Throws<FormatException>(() => new CronExpression("0/ 0 8-18 ? * 2-6"), "Cron did not validate bad range interval in '0/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));
    }

    [Test]
    public void TestMinRangeIntervalAfterSlash()
    {
        // Test case 1
        var e =
            Assert.Throws<FormatException>(() => new CronExpression("0 /120 8-18 ? * 2-6"), "Cron did not validate bad range interval in '_blank/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 59 : 120"));

        // Test case 2
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0/120 8-18 ? * 2-6"), "Cron did not validate bad range interval in in '0/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 59 : 120"));

        // Test case 3
        e = Assert.Throws<FormatException>(() => new CronExpression("0 / 8-18 ? * 2-6"), "Cron did not validate bad range interval in '_blank/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));

        // Test case 4
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0/ 8-18 ? * 2-6"), "Cron did not validate bad range interval in '0/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));
    }

    [Test]
    public void TestHourRangeIntervalAfterSlash()
    {
        // Test case 1
        var e = Assert.Throws<FormatException>(() => new CronExpression("0 0 /120 ? * 2-6"), "Cron did not validate bad range interval in '_blank/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 23 : 120"));

        // Test case 2
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0/120 ? * 2-6"), "Cron did not validate bad range interval in in '0/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 23 : 120"));

        // Test case 3
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 / ? * 2-6"), "Cron did not validate bad range interval in '_blank/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));

        // Test case 4
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0/ ? * 2-6"), "Cron did not validate bad range interval in '0/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));
    }

    [Test]
    public void TestDayOfMonthRangeIntervalAfterSlash()
    {
        // Test case 1
        var e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 /120 * 2-6"), "Cron did not validate bad range interval in '_blank/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 31 : 120"));

        // Test case 2
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 0/120 * 2-6"), "Cron did not validate bad range interval in in '0/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 31 : 120"));

        // Test case 3
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 / * 2-6"), "Cron did not validate bad range interval in '_blank/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));

        // Test case 4
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 0/ * 2-6"), "Cron did not validate bad range interval in '0/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));
    }

    [Test]
    public void TestMonthRangeIntervalAfterSlash()
    {
        // Test case 1
        var e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? /120 2-6"), "Cron did not validate bad range interval in '_blank/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 12 : 120"));

        // Test case 2
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? 0/120 2-6"), "Cron did not validate bad range interval in in '0/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 12 : 120"));

        // Test case 3
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? / 2-6"), "Cron did not validate bad range interval in '_blank/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));

        // Test case 4
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? 0/ 2-6"), "Cron did not validate bad range interval in '0/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));
    }

    [Test]
    public void TestDayOfWeekRangeIntervalAfterSlash()
    {
        // Test case 1
        var e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? * /120"), "Cron did not validate bad range interval in '_blank/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 7 : 120"));

        // Test case 2
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? * 0/120"), "Cron did not validate bad range interval in in '0/xxx' form");
        Assert.That(e.Message, Is.EqualTo("Increment > 7 : 120"));

        // Test case 3
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? * /"), "Cron did not validate bad range interval in '_blank/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));

        // Test case 4
        e = Assert.Throws<FormatException>(() => new CronExpression("0 0 0 ? * 0/"), "Cron did not validate bad range interval in '0/_blank'");
        Assert.That(e.Message, Is.EqualTo("'/' must be followed by an integer."));
    }

    [Test]
    public void TestInvalidCharactersAfterAsterisk()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CronExpression.IsValidExpression("* * * ? * *A&/5:"), Is.False);
            Assert.That(CronExpression.IsValidExpression("* * * ? *14 "), Is.False);
            Assert.That(CronExpression.IsValidExpression(" * * ? *A&/5 *"), Is.False);
            Assert.That(CronExpression.IsValidExpression("* * ? */5 *"), Is.False);
            Assert.That(CronExpression.IsValidExpression("* * ? */52 *"), Is.False);

            Assert.That(CronExpression.IsValidExpression("0 0/30 * * * ?"), Is.True);
            Assert.That(CronExpression.IsValidExpression("0 0/1 * * * ?"), Is.True);
            Assert.That(CronExpression.IsValidExpression("0 0/30 * * */2 ?"), Is.True);
        });
    }

    [Test]
    public void TestExtraCharactersAfterWeekDay()
    {
        Assert.That(CronExpression.IsValidExpression("0 0 15 ? * FRI*"), Is.False);
    }

    [Test]
    public void TestHourRangeAndSlash()
    {
        CronExpression.ValidateExpression("0 0 18-21/1 ? * MON,TUE,WED,THU,FRI,SAT,SUN");
    }

    [Test]
    [Explicit]
    public void PerformanceTest()
    {
        var quartz = new CronExpression("* * * * * ?");

        var sw = new Stopwatch();
        sw.Start();

        DateTimeOffset? next = new DateTimeOffset(2012, 1, 1, 0, 0, 0, TimeSpan.Zero);

        for (var i = 0; i < 1000000; i++)
        {
            next = quartz.GetNextValidTimeAfter(next.Value);

            if (next is null)
                break;
        }

        Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);
    }

    [Test]
    public void CanGetHashCode()
    {
        var expression = new CronExpression("0 15 15 5 11 ?");
        var expression2 = new CronExpression("0 15 15 5 11 ?");
        Assert.That(expression.GetHashCode(), Is.EqualTo(expression2.GetHashCode()));
    }

    [Test]
    public void CanGetExpressionSummary()
    {
        var expression = new CronExpression("0 15 15 5 11 ?");
        var sut = expression.GetExpressionSummary();
        Assert.That(sut, Is.EqualTo(
            @"seconds: 0
minutes: 15
hours: 15
daysOfMonth: 5
months: 11
daysOfWeek: ?
lastdayOfWeek: False
nearestWeekday: False
NthDayOfWeek: 0
lastdayOfMonth: False
calendardayOfWeek: False
calendardayOfMonth: False
years: *
"));
    }

    [TestCase("OCT", 10)]
    [TestCase("NOV", 11)]
    [TestCase("DEC", 12)]
    public void GivenMonthAbbreviation_ShouldGetTimeAfter(string monthAbbr, int monthNumber)
    {
        var expression = $"0 0 0 1 {monthAbbr} ? *";

        CronExpression ce = new(expression) { TimeZone = TimeZoneInfo.Utc };
        var startTime = new DateTimeOffset(2024, 7, 22, 12, 0, 0, TimeSpan.Zero);
        var expectedTimeAfter = new DateTimeOffset(2024, monthNumber, 1, 0, 0, 0, TimeSpan.Zero);

        DateTimeOffset? actualTimeAfter = ce.GetTimeAfter(startTime);

        Assert.That(actualTimeAfter, Is.EqualTo(expectedTimeAfter));
    }

    [TestCaseSource(typeof(CronTestScenarios), nameof(CronTestScenarios.TestCases))]
    public void CronExpressionReturnsExpectedNextFireTime(CronExpression cronExpression, DateTimeOffset timeAfterDate, DateTimeOffset expectedNextFireTime)
    {
        DateTimeOffset? nextFireTime = cronExpression.GetTimeAfter(timeAfterDate);
        Assert.That(nextFireTime.Value.Date, Is.EqualTo(expectedNextFireTime.Date));
    }
}


public class CronTestScenarios
{
    static IEnumerable<TestCaseProps> TestCaseData =>
    [
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 15W * ?"){ TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2024, 5, 15, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2024, 6, 14, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on Weekday 15th Every Month - 2024-06-15 is a Sat, schedule should be Fri 14th"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 15W * ?") { TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2024, 8, 15, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2024, 9, 16, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on Weekday 15th Every Month - 2024-09-15 is a Sunday, expect schedule to be Mon 16th"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 15W * ?"){ TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2023, 12, 15, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2024, 1, 15, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on Weekday 15th Every Month - 2024-01-15 is Monday, should run on Monday"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 31W * ?") { TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2025, 1, 31, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2025, 2, 28, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Test that next fire time to be in next month with less days in month - Issue #2330"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 LW * ?"){ TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2023, 2, 28, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2023, 3, 31, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on last weekday of the month - 2023-03-31 is a Friday"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 L-2 * ?") { TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2023, 4, 28, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2023, 5, 29, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on the second-to-last day of the month"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 ? * 6L") { TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2023, 6, 24, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2023, 6, 30, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on the last Friday of the month - 2023-06-30 is the last Friday"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 ? * 6#3") { TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2023, 7, 21, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2023, 8, 18, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on the third Friday of the month"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 ? * 2/2"){ TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2023, 9, 5, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2023, 9, 6, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run every second day (/2) starting Monday (2)"
        },
        new TestCaseProps
        {
            CronExpression = new CronExpression("0 0 12 1W * ?") { TimeZone = TimeZoneInfo.Utc },
            TimeAfterDate = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero),
            ExpectedNextFireTime = new DateTimeOffset(2023, 10, 2, 12, 0, 0, TimeSpan.Zero),
            TestCase = "Run on the first weekday of the month - 2023-10-01 is a Sunday, expect schedule to be Mon 2nd"
        }
    ];

    public static IEnumerable TestCases =>
        TestCaseData.Select(model => new TestCaseData(model.CronExpression, model.TimeAfterDate, model.ExpectedNextFireTime));


    class TestCaseProps
    {
        public CronExpression CronExpression { get; set; }

        public DateTimeOffset TimeAfterDate { get; set; }

        public DateTimeOffset ExpectedNextFireTime { get; set; }

        public string TestCase { get; set; }
    }
}
