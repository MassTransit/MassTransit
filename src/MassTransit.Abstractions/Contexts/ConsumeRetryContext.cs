namespace MassTransit
{
    using System.Threading.Tasks;


    public interface ConsumeRetryContext
    {
        /// <summary>
        /// The retry attempt in progress, or zero if this is the first time through
        /// </summary>
        int RetryAttempt { get; }

        /// <summary>
        /// The number of retries that have already been attempted, note that this is zero
        /// on the first retry attempt
        /// </summary>
        int RetryCount { get; }

        TContext CreateNext<TContext>(RetryContext retryContext)
            where TContext : class, ConsumeRetryContext;

        Task NotifyPendingFaults();
    }
}
