namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// The base context of a retry
    /// </summary>
    public interface RetryContext
    {
        /// <summary>
        /// Canceled when the retry should be canceled (not the same as if the underlying context
        /// is canceled, which is different). This can be used to cancel retry, but not the operation
        /// itself.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// The exception that originally caused the retry to be initiated
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// The retry attempt currently being attempted (should be 1 > than RetryCount)
        /// </summary>
        int RetryAttempt { get; }

        /// <summary>
        /// The number of retries which were attempted beyond the initial attempt
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        /// The time to wait before the next retry attempt
        /// </summary>
        TimeSpan? Delay { get; }

        /// <summary>
        /// The context type of the retry context
        /// </summary>
        Type ContextType { get; }

        /// <summary>
        /// Called after the retry attempt has failed
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task RetryFaulted(Exception exception);

        /// <summary>
        /// Called before the retry attempt is performed
        /// </summary>
        /// <returns></returns>
        Task PreRetry();
    }


    /// <summary>
    /// The retry context, with the specified context type
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    public interface RetryContext<TContext> :
        RetryContext
        where TContext : class
    {
        /// <summary>
        /// The context being managed by the retry policy
        /// </summary>
        TContext Context { get; }

        /// <summary>
        /// Determines if the exception can be retried
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        /// <param name="retryContext">The retry context for the retry</param>
        /// <returns>True if the task should be retried</returns>
        bool CanRetry(Exception exception, out RetryContext<TContext> retryContext);
    }
}
