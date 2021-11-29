namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Futures;
    using Initializers;
    using SagaStateMachine;


    public class FutureRequestConfigurator<TCommand, TResult, TFault, TInput, TRequest> :
        FutureRequestHandle<TCommand, TResult, TFault, TRequest>,
        IFutureRequestConfigurator<TFault, TInput, TRequest>,
        ISpecification
        where TCommand : class
        where TResult : class
        where TFault : class
        where TInput : class
        where TRequest : class
    {
        readonly IFutureStateMachineConfigurator _configurator;
        readonly FutureFault<TCommand, TFault, Fault<TRequest>> _fault;
        readonly FutureRequest<TInput, TRequest> _request;

        public FutureRequestConfigurator(IFutureStateMachineConfigurator configurator, Event<Fault<TRequest>> faulted)
        {
            _configurator = configurator;

            Faulted = faulted;

            _request = new FutureRequest<TInput, TRequest>();
            _fault = new FutureFault<TCommand, TFault, Fault<TRequest>>();
        }

        public PendingFutureIdProvider<TRequest> PendingRequestIdProvider
        {
            get => _request.PendingRequestIdProvider;
            private set => _request.PendingRequestIdProvider = value;
        }

        public Event<Fault<TRequest>> Faulted { get; }

        public FutureResponseHandle<TCommand, TResult, TFault, TRequest, TResponse>
            OnResponseReceived<TResponse>(Action<IFutureResponseConfigurator<TResult, TResponse>> configure)
            where TResponse : class
        {
            var response = new FutureResponseConfigurator<TCommand, TResult, TFault, TRequest, TResponse>(_configurator, this);

            configure?.Invoke(response);

            Validate().ThrowIfContainsFailure($"The future request response was not configured correctly: {TypeCache.GetShortName(GetType())}");

            if (response.PendingResponseIdProvider != null)
                _configurator.CompletePendingRequest(response.Completed, response.PendingResponseIdProvider);
            else
                _configurator.SetResult(response.Completed, response.SetResult);

            return response;
        }

        public Uri RequestAddress
        {
            set { _request.AddressProvider = context => value; }
        }

        public void SetRequestAddressProvider(RequestAddressProvider<TInput> provider)
        {
            _request.AddressProvider = provider;
        }

        public void UsingRequestFactory(EventMessageFactory<FutureState, TInput, TRequest> factoryMethod)
        {
            _request.Factory = MessageFactory<TRequest>.Create(factoryMethod);
        }

        public void UsingRequestFactory(AsyncEventMessageFactory<FutureState, TInput, TRequest> factoryMethod)
        {
            _request.Factory = MessageFactory<TRequest>.Create(factoryMethod);
        }

        public void UsingRequestInitializer(InitializerValueProvider<TInput> valueProvider)
        {
            Task<SendTuple<TRequest>> Factory(BehaviorContext<FutureState, TInput> context)
            {
                return MessageInitializerCache<TRequest>.InitializeMessage(context, valueProvider(context), new object[] { context.Message });
            }

            _request.Factory = MessageFactory<TRequest>.Create((Func<BehaviorContext<FutureState, TInput>, Task<SendTuple<TRequest>>>)Factory);
        }

        public void TrackPendingRequest(PendingFutureIdProvider<TRequest> provider)
        {
            PendingRequestIdProvider = provider;
        }

        public void OnRequestFaulted(Action<IFutureFaultConfigurator<TFault, Fault<TRequest>>> configure)
        {
            var configurator = new FutureFaultConfigurator<TCommand, TFault, Fault<TRequest>>(_fault);

            configure?.Invoke(configurator);
        }

        public void WhenFaulted(Func<EventActivityBinder<FutureState, Fault<TRequest>>, EventActivityBinder<FutureState, Fault<TRequest>>> configure)
        {
            _configurator.DuringAnyWhen(Faulted, configure);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _request.Validate();
        }

        public Task Send(BehaviorContext<FutureState, TInput> context)
        {
            return context.Message != null
                ? _request.SendRequest(context)
                : Task.CompletedTask;
        }

        public Task Send(BehaviorContext<FutureState, TCommand> context, TInput data)
        {
            return data != null
                ? _request.SendRequest(context.CreateProxy(MessageEvent<TInput>.Instance, data))
                : Task.CompletedTask;
        }

        public Task SendRange(BehaviorContext<FutureState, TCommand> context, IEnumerable<TInput> inputs)
        {
            return inputs != null
                ? Task.WhenAll(inputs.Select(input => Send(context, input)))
                : Task.CompletedTask;
        }

        public Task SetFaulted(BehaviorContext<FutureState, Fault<TRequest>> context)
        {
            return _fault.SetFaulted(context);
        }
    }
}
