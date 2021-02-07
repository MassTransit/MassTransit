namespace MassTransit.Futures.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Automatonymous.Binders;
    using Endpoints;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Internals;
    using MassTransit.Configurators;
    using Util;


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
        RequestAddressProvider<TInput> _addressProvider;
        IRequestEndpoint<TInput, TRequest> _requestEndpoint;

        public FutureRequestConfigurator(IFutureStateMachineConfigurator configurator, Event<Fault<TRequest>> faulted)
        {
            _configurator = configurator;

            Faulted = faulted;

            _addressProvider = FutureConfiguratorHelpers.PublishAddressProvider;
            _fault = new FutureFault<TCommand, TFault, Fault<TRequest>>();
            _requestEndpoint =
                new InitializerRequestEndpoint<TInput, TRequest>(_addressProvider, PendingRequestIdProvider, FutureConfiguratorHelpers.DefaultProvider);
        }

        public PendingIdProvider<TRequest> PendingRequestIdProvider { get; private set; }

        public Event<Fault<TRequest>> Faulted { get; }

        public FutureResponseHandle<TCommand, TResult, TFault, TRequest, TResponse>
            OnResponseReceived<TResponse>(Action<IFutureResponseConfigurator<TResult, TResponse>> configure)
            where TResponse : class
        {
            var response = new FutureResponseConfigurator<TCommand, TResult, TFault, TRequest, TResponse>(_configurator, this);

            configure?.Invoke(response);

            BusConfigurationResult.CompileResults(response.Validate(),
                $"The future request response was not configured correctly: {TypeCache.GetShortName(GetType())}");

            if (response.PendingResponseIdProvider != null)
                _configurator.CompletePendingRequest(response.Completed, response.PendingResponseIdProvider);
            else
                _configurator.SetResult(response.Completed, response.SetResult);

            return response;
        }

        public Uri RequestAddress
        {
            set
            {
                _addressProvider = context => value;
                _requestEndpoint.RequestAddressProvider = _addressProvider;
            }
        }

        public void SetRequestAddressProvider(RequestAddressProvider<TInput> provider)
        {
            _addressProvider = provider;
            _requestEndpoint.RequestAddressProvider = _addressProvider;
        }

        public void UsingRequestFactory(FutureMessageFactory<TInput, TRequest> factoryMethod)
        {
            Task<TRequest> AsyncFactoryMethod(FutureConsumeContext<TInput> context)
            {
                return Task.FromResult(factoryMethod(context));
            }

            _requestEndpoint = new FactoryRequestEndpoint<TInput, TRequest>(_addressProvider, PendingRequestIdProvider, AsyncFactoryMethod);
        }

        public void UsingRequestFactory(AsyncFutureMessageFactory<TInput, TRequest> factoryMethod)
        {
            _requestEndpoint = new FactoryRequestEndpoint<TInput, TRequest>(_addressProvider, PendingRequestIdProvider, factoryMethod);
        }

        public void UsingRequestInitializer(InitializerValueProvider<TInput> valueProvider)
        {
            _requestEndpoint = new InitializerRequestEndpoint<TInput, TRequest>(_addressProvider, PendingRequestIdProvider, valueProvider);
        }

        public void TrackPendingRequest(PendingIdProvider<TRequest> provider)
        {
            PendingRequestIdProvider = provider;
            _requestEndpoint.PendingIdProvider = provider;
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
            if (_addressProvider == null)
                yield return this.Failure("DestinationAddressProvider", "must not be null");
            if (_requestEndpoint == null)
                yield return this.Failure("Command", "Factory", "Init or Create must be configured");
        }

        public Task Send(BehaviorContext<FutureState, TInput> context)
        {
            return context.Data != null
                ? _requestEndpoint.SendCommand(context.CreateFutureConsumeContext(context.Data))
                : TaskUtil.Completed;
        }

        public Task Send(BehaviorContext<FutureState, TCommand> context, TInput data)
        {
            return data != null
                ? _requestEndpoint.SendCommand(context.CreateFutureConsumeContext(data))
                : TaskUtil.Completed;
        }

        public Task SendRange(BehaviorContext<FutureState, TCommand> context, IEnumerable<TInput> inputs)
        {
            return inputs != null
                ? Task.WhenAll(inputs.Select(input => Send(context, input)))
                : TaskUtil.Completed;
        }

        public Task SetFaulted(BehaviorContext<FutureState, Fault<TRequest>> context)
        {
            return _fault.SetFaulted(context);
        }
    }
}
