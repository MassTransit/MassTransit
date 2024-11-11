#nullable enable
namespace MassTransit.RabbitMqTransport.Configuration;

using System;
using RabbitMQ.Client;


public class RabbitMqStreamConfigurator :
    IRabbitMqStreamConfigurator
{
    readonly RabbitMqReceiveSettings _settings;

    public RabbitMqStreamConfigurator(RabbitMqReceiveSettings settings)
    {
        _settings = settings;
    }

    public long MaxLength
    {
        set => _settings.QueueArguments[Headers.XMaxLengthInBytes] = value;
    }

    public TimeSpan MaxAge
    {
        set
        {
            string? text = null;
            if (value.TotalDays >= 1)
                text = $"{value.TotalDays:F0}D";
            else if (value.TotalHours >= 1)
                text = $"{value.TotalHours:F0}h";
            else if (value.TotalMinutes >= 1)
                text = $"{value.TotalMinutes:F0}m";
            else if (value.TotalSeconds >= 1)
                text = $"{value.TotalSeconds:F0}s";

            _settings.QueueArguments[Headers.XMaxAge] = text;
        }
    }

    public long MaxSegmentSize
    {
        set => _settings.QueueArguments[Headers.XStreamMaxSegmentSizeInBytes] = value;
    }

    public string Filter
    {
        set => _settings.ConsumeArguments["x-stream-filter"] = value;
    }

    public void FromOffset(long offset)
    {
        _settings.ConsumeArguments[Headers.XStreamOffset] = offset;
    }

    public void FromTimestamp(DateTime timestamp)
    {
        if (timestamp.Kind == DateTimeKind.Local)
            timestamp = timestamp.ToUniversalTime();

        _settings.ConsumeArguments.SetAmqpTimestamp(Headers.XStreamOffset, timestamp);
    }

    public void FromFirst()
    {
        _settings.ConsumeArguments[Headers.XStreamOffset] = "first";
    }

    public void FromLast()
    {
        _settings.ConsumeArguments[Headers.XStreamOffset] = "last";
    }
}
