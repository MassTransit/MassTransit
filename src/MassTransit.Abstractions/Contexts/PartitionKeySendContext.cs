namespace MassTransit;

public interface PartitionKeySendContext
{
    /// <summary>
    /// The partition key for the message (defaults to "")
    /// </summary>
    string? PartitionKey { get; set; }
}
