namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Intercepts the ConsumeContext
    /// </summary>
    public interface IConsumeObserver
    {
        /// <summary>
        /// Called before a message is dispatched to any consumers
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <returns></returns>
        Task PreConsume<T>(ConsumeContext<T> context)
            where T : class;

        /// <summary>
        /// Called after the message has been dispatched to all consumers - note that in the case of an exception
        /// this method is not called, and the DispatchFaulted method is called instead
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PostConsume<T>(ConsumeContext<T> context)
            where T : class;

        /// <summary>
        /// Called after the message has been dispatched to all consumers when one or more exceptions have occurred
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class;
    }
}
