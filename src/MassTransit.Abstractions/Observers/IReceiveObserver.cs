namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// An observer that can monitor a receive endpoint to track message consumption at the
    /// endpoint level.
    /// </summary>
    public interface IReceiveObserver
    {
        /// <summary>
        /// Called when a message has been delivered by the transport is about to be received by the endpoint
        /// </summary>
        /// <param name="context">The receive context of the message</param>
        /// <returns></returns>
        Task PreReceive(ReceiveContext context);

        /// <summary>
        /// Called when the message has been received and acknowledged on the transport
        /// </summary>
        /// <param name="context">The receive context of the message</param>
        /// <returns></returns>
        Task PostReceive(ReceiveContext context);

        /// <summary>
        /// Called when a message has been consumed by a consumer
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message consume context</param>
        /// <param name="duration">The consumer duration</param>
        /// <param name="consumerType">The consumer type</param>
        /// <returns></returns>
        Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class;

        /// <summary>
        /// Called when a message being consumed produced a fault
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message consume context</param>
        /// <param name="duration">The consumer duration</param>
        /// <param name="consumerType">The consumer type</param>
        /// <param name="exception">The exception from the consumer</param>
        /// <returns></returns>
        Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class;

        /// <summary>
        /// Called when the transport receive faults
        /// </summary>
        /// <param name="context">The receive context of the message</param>
        /// <param name="exception">The exception that was thrown</param>
        /// <returns></returns>
        Task ReceiveFault(ReceiveContext context, Exception exception);
    }
}
