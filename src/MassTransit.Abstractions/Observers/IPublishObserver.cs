namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Observes messages as they are published via a publish endpoint. These should not be used to intercept or
    /// filter messages, in that case a filter should be created and registered on the transport.
    /// </summary>
    public interface IPublishObserver
    {
        /// <summary>
        /// Called before the message is sent to the transport
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <returns></returns>
        Task PrePublish<T>(PublishContext<T> context)
            where T : class;

        /// <summary>
        /// Called after the message is sent to the transport (and confirmed by the transport if supported)
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <returns></returns>
        Task PostPublish<T>(PublishContext<T> context)
            where T : class;

        /// <summary>
        /// Called when the message fails to send to the transport, including the exception that was thrown
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <param name="exception">The exception from the transport</param>
        /// <returns></returns>
        Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class;
    }
}
