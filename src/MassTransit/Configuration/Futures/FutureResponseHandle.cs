namespace MassTransit
{
    public interface FutureResponseHandle<out TCommand, TResult, TFault, TRequest, out TResponse> :
        FutureRequestHandle<TCommand, TResult, TFault, TRequest>
        where TCommand : class
        where TResult : class
        where TFault : class
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// The Response Completed event
        /// </summary>
        Event<TResponse> Completed { get; }
    }
}
