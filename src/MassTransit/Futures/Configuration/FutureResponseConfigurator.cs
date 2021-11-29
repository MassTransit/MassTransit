namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Futures;


    public class FutureResponseConfigurator<TCommand, TResult, TFault, TRequest, TResponse> :
        FutureResponseHandle<TCommand, TResult, TFault, TRequest, TResponse>,
        IFutureResponseConfigurator<TResult, TResponse>,
        ISpecification
        where TCommand : class
        where TResult : class
        where TFault : class
        where TRequest : class
        where TResponse : class
    {
        readonly IFutureStateMachineConfigurator _configurator;
        readonly FutureRequestHandle<TCommand, TResult, TFault, TRequest> _request;
        FutureResult<TCommand, TResult, TResponse> _result;

        public FutureResponseConfigurator(IFutureStateMachineConfigurator configurator, FutureRequestHandle<TCommand, TResult, TFault, TRequest> request)
        {
            _configurator = configurator;
            _request = request;

            Completed = configurator.CreateResponseEvent<TResponse>();
        }

        public PendingFutureIdProvider<TResponse> PendingResponseIdProvider { get; private set; }

        public Event<TResponse> Completed { get; }

        public Event<Fault<TRequest>> Faulted => _request.Faulted;

        public FutureResponseHandle<TCommand, TResult, TFault, TRequest, T> OnResponseReceived<T>(
            Action<IFutureResponseConfigurator<TResult, T>> configure = default)
            where T : class
        {
            return _request.OnResponseReceived(configure);
        }

        public void CompletePendingRequest(PendingFutureIdProvider<TResponse> provider)
        {
            PendingResponseIdProvider = provider;
        }

        public void WhenReceived(Func<EventActivityBinder<FutureState, TResponse>, EventActivityBinder<FutureState, TResponse>> configure)
        {
            _configurator.DuringAnyWhen(Completed, configure);
        }

        public void SetCompletedUsingFactory(EventMessageFactory<FutureState, TResponse, TResult> factoryMethod)
        {
            GetResultConfigurator().SetCompletedUsingFactory(factoryMethod);
        }

        public void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TResponse, TResult> factoryMethod)
        {
            GetResultConfigurator().SetCompletedUsingFactory(factoryMethod);
        }

        public void SetCompletedUsingInitializer(InitializerValueProvider<TResponse> valueProvider)
        {
            GetResultConfigurator().SetCompletedUsingInitializer(valueProvider);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public Task SetResult(BehaviorContext<FutureState, TResponse> context)
        {
            return _result.SetResult(context);
        }

        IFutureResultConfigurator<TResult, TResponse> GetResultConfigurator()
        {
            _result ??= new FutureResult<TCommand, TResult, TResponse>();

            return new FutureResultConfigurator<TCommand, TResult, TResponse>(_result);
        }
    }
}
