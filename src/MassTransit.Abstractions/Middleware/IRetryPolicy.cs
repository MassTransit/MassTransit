namespace MassTransit
{
    using System;


    /// <summary>
    /// A retry policy determines how exceptions are handled, and whether or not the
    /// remaining filters should be retried
    /// </summary>
    public interface IRetryPolicy :
        IProbeSite
    {
        /// <summary>
        /// Creates a retry policy context for the retry, which initiates the exception tracking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        RetryPolicyContext<T> CreatePolicyContext<T>(T context)
            where T : class, PipeContext;

        /// <summary>
        /// If the retry policy handles the exception, should return true
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        bool IsHandled(Exception exception);
    }
}
