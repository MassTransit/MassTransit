namespace MassTransit
{
    using System.Threading.Tasks;


    public interface IRetryObserver
    {
        /// <summary>
        /// Called before a message is dispatched to any consumers
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <returns></returns>
        Task PostCreate<T>(RetryPolicyContext<T> context)
            where T : class, PipeContext;

        /// <summary>
        /// Called after a fault has occurred, but will be retried
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PostFault<T>(RetryContext<T> context)
            where T : class, PipeContext;

        /// <summary>
        /// Called immediately before an exception will be retried
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PreRetry<T>(RetryContext<T> context)
            where T : class, PipeContext;

        /// <summary>
        /// Called when the retry filter is no longer going to retry, and the context is faulted.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task RetryFault<T>(RetryContext<T> context)
            where T : class, PipeContext;

        /// <summary>
        /// Called when the retry filter retried at least once, and the context completed successfully.
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task RetryComplete<T>(RetryContext<T> context)
            where T : class, PipeContext;
    }
}
