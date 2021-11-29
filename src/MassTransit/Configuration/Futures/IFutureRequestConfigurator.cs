namespace MassTransit
{
    using System;
    using Futures;


    public interface IFutureRequestConfigurator<TFault, out TInput, TRequest>
        where TFault : class
        where TInput : class
        where TRequest : class
    {
        /// <summary>
        /// Set the request destination address. If not specified, the request will be published.
        /// </summary>
        Uri RequestAddress { set; }

        /// <summary>
        /// Set the request destination address dynamically using the provider
        /// </summary>
        /// <param name="provider"></param>
        void SetRequestAddressProvider(RequestAddressProvider<TInput> provider);

        /// <summary>
        /// Create the request using a factory method.
        /// </summary>
        /// <param name="factoryMethod">Returns the request message</param>
        void UsingRequestFactory(EventMessageFactory<FutureState, TInput, TRequest> factoryMethod);

        /// <summary>
        /// Create the request using an asynchronous factory method.
        /// </summary>
        /// <param name="factoryMethod">Returns the request message</param>
        void UsingRequestFactory(AsyncEventMessageFactory<FutureState, TInput, TRequest> factoryMethod);

        /// <summary>
        /// Create the request using a message initializer. The initiating command is also used to initialize
        /// request properties prior to apply the values specified.
        /// </summary>
        /// <param name="valueProvider">Returns an object of values to initialize the request</param>
        void UsingRequestInitializer(InitializerValueProvider<TInput> valueProvider);

        /// <summary>
        /// If specified, the request is added to the pending results, using the identifier returned by the
        /// provider. A subsequent result with a matching identifier will complete the pending result.
        /// </summary>
        /// <param name="provider">Provides the identifier from the request</param>
        void TrackPendingRequest(PendingFutureIdProvider<TRequest> provider);

        /// <summary>
        /// Configure what happens when the request faults
        /// </summary>
        /// <param name="configure"></param>
        void OnRequestFaulted(Action<IFutureFaultConfigurator<TFault, Fault<TRequest>>> configure);

        /// <summary>
        /// Add activities to the state machine that are executed when the request faults
        /// </summary>
        /// <param name="configure"></param>
        void WhenFaulted(Func<EventActivityBinder<FutureState, Fault<TRequest>>, EventActivityBinder<FutureState, Fault<TRequest>>> configure);
    }
}
