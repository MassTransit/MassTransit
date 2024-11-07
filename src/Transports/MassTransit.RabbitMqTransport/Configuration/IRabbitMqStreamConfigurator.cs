#nullable enable
namespace MassTransit;

using System;


public interface IRabbitMqStreamConfigurator
{
    /// <summary>
    /// Set the maximum length of the stream, in bytes
    /// </summary>
    long MaxLength { set; }

    /// <summary>
    /// Set the maximum age of messages in the stream
    /// </summary>
    TimeSpan MaxAge { set; }

    /// <summary>
    /// Set the maximum segment size for the stream
    /// </summary>
    long MaxSegmentSize { set; }

    /// <summary>
    /// Set the stream filter value for the consumer
    /// </summary>
    string Filter { set; }

    /// <summary>
    /// Begin consuming messages from the specified offset
    /// </summary>
    /// <param name="offset"></param>
    void FromOffset(long offset);

    /// <summary>
    /// Begin consuming messages from the specified timestamp
    /// </summary>
    /// <param name="timestamp"></param>
    void FromTimestamp(DateTime timestamp);

    /// <summary>
    /// Begin consuming messages from the first message in the stream
    /// </summary>
    void FromFirst();

    /// <summary>
    /// Begin consuming messages from the last message in the stream
    /// </summary>
    void FromLast();
}
