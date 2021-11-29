namespace MassTransit
{
    using System;
    using Futures;


    public interface IFutureResponseConfigurator<TResult, TResponse> :
        IFutureResultConfigurator<TResult, TResponse>
        where TResult : class
        where TResponse : class
    {
        /// <summary>
        /// If specified, the identifier is used to complete a pending result and the result will be stored
        /// in the future.
        /// </summary>
        /// <param name="provider">Provides the identifier from the request</param>
        void CompletePendingRequest(PendingFutureIdProvider<TResponse> provider);

        /// <summary>
        /// Add activities to the state machine that are executed when the response is received
        /// </summary>
        /// <param name="configure"></param>
        void WhenReceived(Func<EventActivityBinder<FutureState, TResponse>, EventActivityBinder<FutureState, TResponse>> configure);
    }
}
