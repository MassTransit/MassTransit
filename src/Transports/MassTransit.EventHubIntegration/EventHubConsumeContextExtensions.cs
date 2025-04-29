#nullable enable
namespace MassTransit;

using System;


public static class EventHubConsumeContextExtensions
{
    [Obsolete("The Event Hubs service does not guarantee a numeric offset for all resource configurations.  Please use 'OffsetString' instead.")]
    public static long? Offset(this ConsumeContext context)
    {
        return context.TryGetPayload(out EventHubConsumeContext? consumeContext) ? consumeContext.Offset : null;
    }

    public static string? OffsetString(this ConsumeContext context)
    {
        return context.TryGetPayload(out EventHubConsumeContext? consumeContext) ? consumeContext.OffsetString : null;
    }

    public static long? SequenceNumber(this ConsumeContext context)
    {
        return context.TryGetPayload(out EventHubConsumeContext? consumeContext) ? consumeContext.SequenceNumber : null;
    }
}
