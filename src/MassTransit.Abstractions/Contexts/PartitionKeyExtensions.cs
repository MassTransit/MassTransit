namespace MassTransit;

using System;


public static class PartitionKeyExtensions
{
    public static string? PartitionKey(this ConsumeContext context)
    {
        return context.TryGetPayload(out PartitionKeyConsumeContext? consumeContext) ? consumeContext.PartitionKey : string.Empty;
    }

    public static string? PartitionKey(this SendContext context)
    {
        return context.TryGetPayload(out PartitionKeySendContext? sendContext) ? sendContext.PartitionKey : string.Empty;
    }

    /// <summary>
    /// Sets the routing key for this message
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routingKey">The routing key for this message</param>
    public static void SetPartitionKey(this SendContext context, string? routingKey)
    {
        if (!context.TryGetPayload(out PartitionKeySendContext? sendContext))
            throw new ArgumentException("The SendPartitionKeyContext was not available");

        sendContext.PartitionKey = routingKey;
    }

    /// <summary>
    /// Sets the routing key for this message
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routingKey">The routing key for this message</param>
    public static bool TrySetPartitionKey(this SendContext context, string? routingKey)
    {
        if (!context.TryGetPayload(out PartitionKeySendContext? sendContext))
            return false;

        sendContext.PartitionKey = routingKey;
        return true;
    }
}
