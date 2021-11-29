namespace MassTransit.Middleware.InMemoryOutbox
{
    /// <summary>
    /// Specify options to the outbox when sending, so the request client, scheduler, etc. can clear
    /// the outbox without being blocked.
    /// </summary>
    public interface OutboxSendContext
    {
        /// <summary>
        /// If true, the outbox should be skipped for this message
        /// </summary>
        bool SkipOutbox { get; }
    }
}
