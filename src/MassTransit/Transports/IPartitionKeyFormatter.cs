namespace MassTransit.Transports;

public interface IPartitionKeyFormatter
{
    /// <summary>
    /// Format the partition key to be used by the transport, if supported
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="context">The message send context</param>
    /// <returns>The routing key to specify in the transport</returns>
    string FormatPartitionKey<T>(SendContext<T> context)
        where T : class;
}
