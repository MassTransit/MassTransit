namespace MassTransit
{
    using System;


    public interface FutureRequestHandle<out TCommand, TResult, TFault, TRequest>
        where TCommand : class
        where TResult : class
        where TFault : class
        where TRequest : class
    {
        /// <summary>
        /// The Request Faulted event
        /// </summary>
        Event<Fault<TRequest>> Faulted { get; }

        /// <summary>
        /// Handle the response type specified, and configure the response behavior
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The response type</typeparam>
        /// <returns></returns>
        FutureResponseHandle<TCommand, TResult, TFault, TRequest, T> OnResponseReceived<T>(Action<IFutureResponseConfigurator<TResult, T>> configure = null)
            where T : class;
    }
}
