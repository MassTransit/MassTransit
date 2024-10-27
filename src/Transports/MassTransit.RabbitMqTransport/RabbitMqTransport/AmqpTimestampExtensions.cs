namespace MassTransit.RabbitMqTransport;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Internals;
using RabbitMQ.Client;


public static class AmqpTimestampExtensions
{
    /// <summary>
    /// Assign a dictionary key to the value of the <paramref name="timestamp" /> as an <seealso cref="AmqpTimestamp" />.
    /// </summary>
    /// <param name="dictionary">The dictionary</param>
    /// <param name="key">The dictionary key</param>
    /// <param name="timestamp">The timestamp</param>
    public static void SetAmqpTimestamp(this IDictionary<string, object> dictionary, string key, DateTime timestamp)
    {
        dictionary[key] = TryConvert(timestamp, out AmqpTimestamp? result)
            ? result
            : timestamp.ToString("O");
    }

    static bool TryConvert(DateTime input, [NotNullWhen(true)] out AmqpTimestamp? result)
    {
        if (input >= DateTimeConstants.Epoch)
        {
            var timeSpan = input - DateTimeConstants.Epoch;
            if (timeSpan.TotalSeconds <= long.MaxValue)
            {
                result = new AmqpTimestamp((long)timeSpan.TotalSeconds);
                return true;
            }
        }

        result = default;
        return false;
    }
}
