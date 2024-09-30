namespace MassTransit.Tests;

using JobService.Messages;
using NUnit.Framework;


public class RecurringSchedule_Specs
{
    [Test]
    public void Should_return_every_five_seconds()
    {
        var info = new RecurringJobScheduleInfo();
        info.Every(seconds: 5);

        Assert.That(info.CronExpression, Is.EqualTo("0/5 * * * * *"));
    }

    [Test]
    public void Should_return_every_five_seconds_from_noon_until_one()
    {
        var info = new RecurringJobScheduleInfo();
        info.Every(seconds: 5, hour: 12);

        Assert.That(info.CronExpression, Is.EqualTo("0/5 * 12 * * *"));
    }

    [Test]
    public void Should_return_every_five_seconds_from_noon_until_one_ish()
    {
        var info = new RecurringJobScheduleInfo();
        info.Every(seconds: 5, hour: 12, minute: 15);

        Assert.That(info.CronExpression, Is.EqualTo("0/5 15 12 * * *"));
    }

    [Test]
    public void Should_return_every_ten_minutes()
    {
        var info = new RecurringJobScheduleInfo();
        info.Every(minutes: 10);

        Assert.That(info.CronExpression, Is.EqualTo("0 0/10 * * * *"));
    }
}
