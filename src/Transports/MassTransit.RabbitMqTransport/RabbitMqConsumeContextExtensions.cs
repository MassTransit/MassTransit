#nullable enable
namespace MassTransit;

using System;
using Internals;
using RabbitMQ.Client;


public static class RabbitMqConsumeContextExtensions
{
    public static DateTime? GetRabbitMqTimestamp(this ConsumeContext context)
    {
        if (context.ReceiveContext.TransportHeaders.TryGetHeader(MessageHeaders.TransportSentTime, out object? value))
        {
            if (value is AmqpTimestamp ts)
                return DateTimeConstants.Epoch + TimeSpan.FromMilliseconds(ts.UnixTime);
        }

        return null;
    }
}
