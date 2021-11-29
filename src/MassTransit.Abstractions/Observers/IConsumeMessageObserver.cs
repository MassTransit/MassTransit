namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Intercepts the ConsumeContext
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public interface IConsumeMessageObserver<in T>
        where T : class
    {
        /// <summary>
        /// Called before a message is dispatched to any consumers
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <returns></returns>
        Task PreConsume(ConsumeContext<T> context);

        /// <summary>
        /// Called after the message has been dispatched to all consumers - note that in the case of an exception
        /// this method is not called, and the DispatchFaulted method is called instead
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PostConsume(ConsumeContext<T> context);

        /// <summary>
        /// Called after the message has been dispatched to all consumers when one or more exceptions have occurred
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task ConsumeFault(ConsumeContext<T> context, Exception exception);
    }
}
