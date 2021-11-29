namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Courier.Contracts;
    using Futures;


    public class FutureRoutingSlipConfigurator<TCommand, TResult, TFault, TInput> :
        FutureRoutingSlipHandle,
        IFutureRoutingSlipConfigurator<TResult, TFault, TInput>,
        ISpecification
        where TCommand : class
        where TResult : class
        where TInput : class
        where TFault : class

    {
        readonly IFutureStateMachineConfigurator _configurator;
        IRoutingSlipExecutor<TInput> _executor;
        FutureFault<TCommand, TFault, RoutingSlipFaulted> _fault;
        FutureResult<TCommand, TResult, RoutingSlipCompleted> _result;

        public FutureRoutingSlipConfigurator(IFutureStateMachineConfigurator configurator, Event<RoutingSlipCompleted> routingSlipCompleted,
            Event<RoutingSlipFaulted> routingSlipFaulted)
        {
            _configurator = configurator;
            Completed = routingSlipCompleted;
            Faulted = routingSlipFaulted;

            _executor = new PlanRoutingSlipExecutor<TInput>();

            OnRoutingSlipFaulted(fault => fault.SetFaultedUsingInitializer(context => RoutingSlipFaultedValueProvider(context)));
        }

        public PendingFutureIdProvider<RoutingSlipCompleted> CompletedIdProvider { get; private set; }
        public PendingFutureIdProvider<RoutingSlipFaulted> FaultedIdProvider { get; private set; }

        public Event<RoutingSlipCompleted> Completed { get; }
        public Event<RoutingSlipFaulted> Faulted { get; }

        public void OnRoutingSlipCompleted(Action<IFutureResultConfigurator<TResult, RoutingSlipCompleted>> configure)
        {
            _result ??= new FutureResult<TCommand, TResult, RoutingSlipCompleted>();

            var configurator = new FutureResultConfigurator<TCommand, TResult, RoutingSlipCompleted>(_result);

            configure?.Invoke(configurator);
        }

        public void OnRoutingSlipFaulted(Action<IFutureFaultConfigurator<TFault, RoutingSlipFaulted>> configure)
        {
            _fault ??= new FutureFault<TCommand, TFault, RoutingSlipFaulted>();

            var configurator = new FutureFaultConfigurator<TCommand, TFault, RoutingSlipFaulted>(_fault);

            configure?.Invoke(configurator);
        }

        public void WhenRoutingSlipCompleted(
            Func<EventActivityBinder<FutureState, RoutingSlipCompleted>, EventActivityBinder<FutureState, RoutingSlipCompleted>> configure)
        {
            _configurator.DuringAnyWhen(Completed, configure);
        }

        public void WhenRoutingSlipFaulted(
            Func<EventActivityBinder<FutureState, RoutingSlipFaulted>, EventActivityBinder<FutureState, RoutingSlipFaulted>> configure)
        {
            _configurator.DuringAnyWhen(Faulted, configure);
        }

        public void TrackPendingRoutingSlip()
        {
            _executor.TrackRoutingSlip = true;

            CompletedIdProvider = GetTrackingNumber;
            FaultedIdProvider = GetTrackingNumber;
        }

        public void BuildItinerary(BuildItineraryCallback<TInput> buildItinerary)
        {
            _executor = new BuildRoutingSlipExecutor<TInput>(buildItinerary);
        }

        public void BuildUsingItineraryPlanner()
        {
            _executor = new PlanRoutingSlipExecutor<TInput>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_result != null)
            {
                foreach (var result in _result.Validate())
                    yield return result.WithParentKey("RoutingSlip");
            }
            else
            {
                if (CompletedIdProvider == null)
                    yield return this.Failure("RoutingSlip", "Result", "WhenCompleted or TrackingPendingRoutingSlip must be configured");
                if (FaultedIdProvider == null)
                    yield return this.Failure("RoutingSlip", "Fault", "WhenFaulted or TrackingPendingRoutingSlip must be configured");
            }

            if (_fault != null)
            {
                foreach (var result in _fault.Validate())
                    yield return result.WithParentKey("RoutingSlip");
            }

            if (_executor == null)
                yield return this.Failure("RoutingSlip", "Build", "BuildItinerary or BuildUsingItineraryPlanner must be specified");
        }

        static object RoutingSlipFaultedValueProvider(BehaviorContext<FutureState, RoutingSlipFaulted> context)
        {
            var message = context.GetCommand<TCommand>();

            IEnumerable<ExceptionInfo> exceptions = context.Message.ActivityExceptions.Select(x => x.ExceptionInfo);

            return new
            {
                FaultId = context.MessageId ?? NewId.NextGuid(),
                FaultedMessageId = context.Message.TrackingNumber,
                FaultMessageTypes = MessageTypeCache<TCommand>.MessageTypeNames,
                Host = context.Message.ActivityExceptions.Select(x => x.Host).FirstOrDefault() ?? context.Host,
                context.Message.Timestamp,
                exceptions,
                message
            };
        }

        public bool HasResult(out FutureResult<TCommand, TResult, RoutingSlipCompleted> result)
        {
            result = _result;
            return result != null;
        }

        public bool HasFault(out FutureFault<TCommand, TFault, RoutingSlipFaulted> fault)
        {
            fault = _fault;
            return fault != null;
        }

        public Task Execute(BehaviorContext<FutureState, TInput> context)
        {
            return _executor.Execute(context);
        }

        static Guid GetTrackingNumber(RoutingSlipCompleted message)
        {
            return message.TrackingNumber;
        }

        static Guid GetTrackingNumber(RoutingSlipFaulted message)
        {
            return message.TrackingNumber;
        }
    }
}
