namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IFilterObserver
    {
        /// <summary>
        /// Called before a message is dispatched to any consumers
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <returns></returns>
        Task PreSend<T>(T context)
            where T : class, PipeContext;

        /// <summary>
        /// Called after the message has been dispatched to all consumers - note that in the case of an exception
        /// this method is not called, and the DispatchFaulted method is called instead
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PostSend<T>(T context)
            where T : class, PipeContext;

        /// <summary>
        /// Called after the message has been dispatched to all consumers when one or more exceptions have occurred
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task SendFault<T>(T context, Exception exception)
            where T : class, PipeContext;
    }


    public interface IFilterObserver<in TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Called before a message is dispatched to any consumers
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <returns></returns>
        Task PreSend(TContext context);

        /// <summary>
        /// Called after the message has been dispatched to all consumers - note that in the case of an exception
        /// this method is not called, and the DispatchFaulted method is called instead
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PostSend(TContext context);

        /// <summary>
        /// Called after the message has been dispatched to all consumers when one or more exceptions have occurred
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task SendFault(TContext context, Exception exception);
    }
}
