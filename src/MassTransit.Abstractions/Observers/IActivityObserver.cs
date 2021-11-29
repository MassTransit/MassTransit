namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Courier;


    public interface IActivityObserver
    {
        /// <summary>
        /// Called before a message is dispatched to any consumers
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <returns></returns>
        Task PreExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Called after the message has been dispatched to all consumers - note that in the case of an exception
        /// this method is not called, and the DispatchFaulted method is called instead
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PostExecute<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Called after the message has been dispatched to all consumers when one or more exceptions have occurred
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task ExecuteFault<TActivity, TArguments>(ExecuteActivityContext<TActivity, TArguments> context, Exception exception)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Called before a message is dispatched to any consumers
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <returns></returns>
        Task PreCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;

        /// <summary>
        /// Called after the message has been dispatched to all consumers - note that in the case of an exception
        /// this method is not called, and the DispatchFaulted method is called instead
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task PostCompensate<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;

        /// <summary>
        /// Called after the message has been dispatched to all consumers when one or more exceptions have occurred
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task CompensateFail<TActivity, TLog>(CompensateActivityContext<TActivity, TLog> context, Exception exception)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;
    }
}
