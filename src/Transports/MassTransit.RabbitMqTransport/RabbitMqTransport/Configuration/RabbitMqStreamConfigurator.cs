namespace MassTransit.RabbitMqTransport.Configuration;

using System;
using Initializers.TypeConverters;
using RabbitMQ.Client;


public class RabbitMqStreamConfigurator :
    IRabbitMqStreamConfigurator
{
    static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();

    readonly RabbitMqReceiveSettings _settings;

    public RabbitMqStreamConfigurator(RabbitMqReceiveSettings settings)
    {
        _settings = settings;
    }

    public long MaxLength
    {
        set => _settings.QueueArguments["x-max-length-bytes"] = value;
    }

    public TimeSpan MaxAge
    {
        set => _settings.QueueArguments["x-max-age"] = value;
    }

    public long MaxSegmentSize
    {
        set => _settings.QueueArguments["x-stream-max-segment-size-bytes"] = value;
    }

    public void FromOffset(long offset)
    {
        _settings.ConsumeArguments["x-stream-offset"] = offset;
    }

    public void FromTimestamp(DateTime timestamp)
    {
        if (timestamp.Kind == DateTimeKind.Local)
            timestamp = timestamp.ToUniversalTime();

        if (_dateTimeConverter.TryConvert(timestamp, out long result))
            _settings.ConsumeArguments["x-stream-offset"] = new AmqpTimestamp(result);
        else if (_dateTimeConverter.TryConvert(timestamp, out string text))
            _settings.ConsumeArguments["x-stream-offset"] = text;
    }

    public void FromFirst()
    {
        _settings.ConsumeArguments["x-stream-offset"] = "first";
    }

    public void FromLast()
    {
        _settings.ConsumeArguments["x-stream-offset"] = "last";
    }
}
