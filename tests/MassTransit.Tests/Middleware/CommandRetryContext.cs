namespace MassTransit.Tests.Middleware
{
    public interface CommandRetryContext
    {
        /// <summary>
        /// The retry attempt in progress, or zero if this is the first time through
        /// </summary>
        int RetryAttempt { get; }
    }
}
