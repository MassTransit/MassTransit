namespace MassTransit.Tests.Serialization;

using System;
using MassTransit.Initializers.TypeConverters;
using NUnit.Framework;


[TestFixture]
public class DateTimeConverter_Specs
{
    [Test]
    public void Should_convert_date_time_min_value()
    {
        var converter = new DateTimeTypeConverter();

        var value = DateTime.MinValue;

        Assert.Multiple(() =>
        {
            Assert.That(converter.TryConvert(value, out string text));

            Assert.That(converter.TryConvert(text, out var result));

            Assert.That(result, Is.EqualTo(value));
        });
    }

    [Test]
    public void Should_convert_date_time_min_value_to_offset()
    {
        var converter = new DateTimeTypeConverter();
        var offsetConverter = new DateTimeOffsetTypeConverter();

        var value = DateTime.MinValue.ToUniversalTime();

        Assert.Multiple(() =>
        {
            Assert.That(converter.TryConvert(value, out string text));

            Assert.That(offsetConverter.TryConvert(text, out var result));

            Assert.That(result.UtcDateTime, Is.EqualTo(value));
        });
    }

    [Test]
    public void Should_convert_date_time_offset_min_value()
    {
        var converter = new DateTimeOffsetTypeConverter();

        var value = DateTimeOffset.MinValue;

        Assert.Multiple(() =>
        {
            Assert.That(converter.TryConvert(value, out string text));

            Assert.That(converter.TryConvert(text, out var result));

            Assert.That(result, Is.EqualTo(value));
        });
    }
}
