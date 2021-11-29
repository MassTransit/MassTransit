namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Courier.Contracts;
    using Futures;
    using Futures.Contracts;
    using Serialization;


    /// <summary>
    /// A future is a deterministic, durable service that given a command, executes any number
    /// of requests, routing slips, functions, etc. to produce a result. Once the result has been set,
    /// it is available to any subsequent commands and requests for the result.
    /// </summary>
    /// <typeparam name="TCommand">The command type that creates the future</typeparam>
    /// <typeparam name="TResult">The result type that completes the future</typeparam>
    /// <typeparam name="TFault">The fault type that faults the future</typeparam>
    public abstract class Future<TCommand, TResult, TFault> :
        MassTransitStateMachine<FutureState>,
        IFutureStateMachineConfigurator
        where TCommand : class
        where TResult : class
        where TFault : class
    {
        readonly FutureFault<TFault> _fault = new FutureFault<TFault>();
        readonly FutureResult<TCommand, TResult> _result = new FutureResult<TCommand, TResult>();

        protected Future()
        {
            InstanceState(x => x.CurrentState, WaitingForCompletion, Completed, Faulted);

            Event(() => ResultRequested, e =>
            {
                e.CorrelateById(x => x.Message.CorrelationId);
                e.OnMissingInstance(x => x.Execute(context => throw new FutureNotFoundException(typeof(TCommand), context.Message.CorrelationId)));
                e.ConfigureConsumeTopology = false;
            });

            Initially(
                When(CommandReceived)
                    .InitializeFuture()
                    .TransitionTo(WaitingForCompletion)
            );

            During(WaitingForCompletion,
                When(CommandReceived)
                    .AddSubscription(),
                When(ResultRequested)
                    .AddSubscription()
            );

            During(Completed,
                When(CommandReceived)
                    .RespondAsync(x => GetResult(x)),
                When(ResultRequested)
                    .RespondAsync(x => GetResult(x))
            );

            During(Faulted,
                When(CommandReceived)
                    .RespondAsync(x => GetFault(x)),
                When(ResultRequested)
                    .RespondAsync(x => GetFault(x))
            );

            WhenAnyFaulted(x => x.SetFaultedUsingInitializer(context =>
            {
                var message = context.GetCommand<TCommand>();

                // use supported message types to deserialize results...

                List<Fault> faults = context.Saga.Faults.Select(fault => context.ToObject<Fault>(fault.Value)).ToList();

                var faulted = faults.First();

                ExceptionInfo[] exceptions = faults.SelectMany(fault => fault.Exceptions).ToArray();

                return new
                {
                    faulted.FaultId,
                    faulted.FaultedMessageId,
                    Timestamp = context.Saga.Faulted,
                    Exceptions = exceptions,
                    faulted.Host,
                    faulted.FaultMessageTypes,
                    Message = message
                };
            }));
        }

        // States
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public State WaitingForCompletion { get; protected set; }
        public State Completed { get; protected set; }
        public State Faulted { get; protected set; }

        // ReSharper disable once MemberCanBeProtected.Global
        /// <summary>
        /// Initiates and correlates the command to the future. Subsequent commands received while waiting for completion
        /// are added as subscribers.
        /// </summary>
        public Event<TCommand> CommandReceived { get; protected set; }

        /// <summary>
        /// Used by a Future Reference to get the future's result once completed or fault once faulted.
        /// </summary>
        public Event<Get<TCommand>> ResultRequested { get; protected set; }

        /// <summary>
        /// Configure the initiating command, including correlation, etc.
        /// </summary>
        /// <param name="configure"></param>
        protected void ConfigureCommand(Action<IEventCorrelationConfigurator<FutureState, TCommand>> configure)
        {
            Event(() => CommandReceived, configurator =>
            {
                configure?.Invoke(configurator);
            });
        }

        /// <summary>
        /// Send a request when the future is requested
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="TRequest">The request type to send</typeparam>
        protected FutureRequestHandle<TCommand, TResult, TFault, TRequest>
            SendRequest<TRequest>(Action<IFutureRequestConfigurator<TFault, TCommand, TRequest>> configure = default)
            where TRequest : class
        {
            FutureRequestConfigurator<TCommand, TResult, TFault, TCommand, TRequest> request = CreateFutureRequest(configure);

            Initially(
                When(CommandReceived)
                    .ThenAsync(context => request.Send(context))
            );

            return request;
        }

        /// <summary>
        /// Send a request when the future is requested
        /// </summary>
        /// <param name="inputSelector">Specify an input property from the command to use as the input for the request</param>
        /// <param name="configure"></param>
        /// <typeparam name="TRequest">The request type to send</typeparam>
        /// <typeparam name="TInput">The input type</typeparam>
        protected FutureRequestHandle<TCommand, TResult, TFault, TRequest> SendRequest<TInput, TRequest>(Func<TCommand, TInput> inputSelector,
            Action<IFutureRequestConfigurator<TFault, TInput, TRequest>> configure = default)
            where TInput : class
            where TRequest : class
        {
            FutureRequestConfigurator<TCommand, TResult, TFault, TInput, TRequest> request = CreateFutureRequest(configure);

            Initially(
                When(CommandReceived)
                    .ThenAsync(context => request.Send(context, inputSelector(context.Message)))
            );

            return request;
        }

        /// <summary>
        /// Sends multiple requests when the future is requested, using an enumerable request property as the source
        /// </summary>
        /// <param name="inputSelector"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TRequest">The request type to send</typeparam>
        /// <typeparam name="TInput">The input property type</typeparam>
        protected FutureRequestHandle<TCommand, TResult, TFault, TRequest> SendRequests<TInput, TRequest>(Func<TCommand, IEnumerable<TInput>> inputSelector,
            Action<IFutureRequestConfigurator<TFault, TInput, TRequest>> configure)
            where TInput : class
            where TRequest : class
        {
            FutureRequestConfigurator<TCommand, TResult, TFault, TInput, TRequest> request = CreateFutureRequest(configure);

            Initially(
                When(CommandReceived)
                    .ThenAsync(context => request.SendRange(context, inputSelector(context.Message)))
            );

            return request;
        }

        /// <summary>
        /// Execute a routing slip when the future is requested
        /// </summary>
        /// <param name="configure"></param>
        protected FutureRoutingSlipHandle ExecuteRoutingSlip(Action<IFutureRoutingSlipConfigurator<TResult, TFault, TCommand>> configure)
        {
            FutureRoutingSlipConfigurator<TCommand, TResult, TFault, TCommand> routingSlip = CreateFutureRoutingSlip(configure);

            Initially(
                When(CommandReceived)
                    .ThenAsync(context => routingSlip.Execute(context))
            );

            return routingSlip;
        }

        FutureRequestConfigurator<TCommand, TResult, TFault, TInput, TRequest> CreateFutureRequest<TInput, TRequest>(
            Action<IFutureRequestConfigurator<TFault, TInput, TRequest>> configure)
            where TInput : class
            where TRequest : class
        {
            Event<Fault<TRequest>> requestFaulted = Event<Fault<TRequest>>(FormatEventName<TRequest>() + "Faulted", x =>
            {
                x.CorrelateById(m => RequestIdOrFault(m));
                x.OnMissingInstance(m => m.Execute(context => throw new FutureNotFoundException(GetType(), RequestIdOrDefault(context))));
                x.ConfigureConsumeTopology = false;
            });

            var request = new FutureRequestConfigurator<TCommand, TResult, TFault, TInput, TRequest>(this, requestFaulted);

            configure?.Invoke(request);

            request.Validate().ThrowIfContainsFailure($"The future request was not configured correctly: {TypeCache.GetShortName(GetType())}");

            if (request.PendingRequestIdProvider != null)
                FaultPendingRequest(requestFaulted, request.PendingRequestIdProvider);
            else
                SetFaulted(requestFaulted, request.SetFaulted);

            return request;
        }

        FutureRoutingSlipConfigurator<TCommand, TResult, TFault, TInput> CreateFutureRoutingSlip<TInput>(
            Action<IFutureRoutingSlipConfigurator<TResult, TFault, TInput>> configure)
            where TInput : class
        {
            Event<RoutingSlipCompleted> routingSlipCompleted = Event<RoutingSlipCompleted>(FormatEventName<RoutingSlipCompleted>(), x =>
            {
                x.CorrelateById(m => FutureIdOrFault(m, m.Message.Variables));
                x.OnMissingInstance(m => m
                    .Execute(context => throw new FutureNotFoundException(GetType(), FutureIdOrDefault(context, context.Message.Variables))));
                x.ConfigureConsumeTopology = false;
            });

            Event<RoutingSlipFaulted> routingSlipFaulted = Event<RoutingSlipFaulted>(FormatEventName<RoutingSlipFaulted>(), x =>
            {
                x.CorrelateById(m => FutureIdOrFault(m, m.Message.Variables));
                x.OnMissingInstance(m => m
                    .Execute(context => throw new FutureNotFoundException(GetType(), FutureIdOrDefault(context, context.Message.Variables))));
                x.ConfigureConsumeTopology = false;
            });

            var routingSlip = new FutureRoutingSlipConfigurator<TCommand, TResult, TFault, TInput>(this, routingSlipCompleted, routingSlipFaulted);

            configure?.Invoke(routingSlip);

            routingSlip.Validate().ThrowIfContainsFailure($"The future routing slip was not configured correctly: {TypeCache.GetShortName(GetType())}");

            if (routingSlip.FaultedIdProvider != null)
                FaultPendingRoutingSlip(routingSlipFaulted);
            else
            {
                if (routingSlip.HasFault(out FutureFault<TCommand, TFault, RoutingSlipFaulted> fault))
                    SetFaulted(routingSlipFaulted, fault.SetFaulted);
                else
                    SetFaulted(routingSlipFaulted, _fault.SetFaulted);
            }

            if (routingSlip.CompletedIdProvider != null)
                CompletePending(routingSlipCompleted, routingSlip.CompletedIdProvider);
            else if (routingSlip.HasResult(out FutureResult<TCommand, TResult, RoutingSlipCompleted> result))
                SetResult(routingSlipCompleted, result.SetResult);

            return routingSlip;
        }

        Event<T> IFutureStateMachineConfigurator.CreateResponseEvent<T>()
            where T : class
        {
            Event<T> requestCompleted = Event<T>(FormatEventName<T>(), x =>
            {
                x.CorrelateById(m => RequestIdOrFault(m));
                x.OnMissingInstance(m => m.Execute(context => throw new FutureNotFoundException(GetType(), RequestIdOrDefault(context))));
                x.ConfigureConsumeTopology = false;
            });

            return requestCompleted;
        }

        void IFutureStateMachineConfigurator.CompletePendingRequest<T>(Event<T> requestCompleted, PendingFutureIdProvider<T> pendingIdProvider)
            where T : class
        {
            CompletePending(requestCompleted, pendingIdProvider);
        }

        void IFutureStateMachineConfigurator.DuringAnyWhen<T>(Event<T> whenEvent, Func<EventActivityBinder<FutureState, T>,
            EventActivityBinder<FutureState, T>> configure)
            where T : class
        {
            EventActivityBinder<FutureState, T> binder = When(whenEvent);

            binder = configure?.Invoke(binder) ?? binder;

            DuringAny(binder);
        }

        void CompletePending<T>(Event<T> completedEvent, PendingFutureIdProvider<T> pendingIdProvider)
            where T : class
        {
            DuringAny(
                When(completedEvent)
                    .SetResult(x => pendingIdProvider(x.Message), x => x.Message)
                    .IfElse(context => context.Saga.Completed.HasValue,
                        completed => completed
                            .ThenAsync(context => _result.SetResult(context))
                            .TransitionTo(Completed),
                        notCompleted => notCompleted.If(context => context.Saga.Faulted.HasValue,
                            faulted => faulted
                                .ThenAsync(context => _fault.SetFaulted(context))
                                .TransitionTo(Faulted)))
            );
        }

        public void FaultPendingRequest<T>(Event<Fault<T>> requestFaulted, PendingFutureIdProvider<T> pendingIdProvider)
            where T : class
        {
            DuringAny(
                When(requestFaulted)
                    .SetFault(x => pendingIdProvider(x.Message.Message), x => x.Message)
                    .If(context => context.Saga.Faulted.HasValue,
                        faulted => faulted
                            .ThenAsync(context => _fault.SetFaulted(context))
                            .TransitionTo(Faulted))
            );
        }

        void FaultPendingRoutingSlip(Event<RoutingSlipFaulted> requestFaulted)
        {
            DuringAny(
                When(requestFaulted)
                    .SetFault(x => x.Message)
                    .If(context => context.Saga.Faulted.HasValue,
                        faulted => faulted
                            .ThenAsync(context => _fault.SetFaulted(context))
                            .TransitionTo(Faulted))
            );
        }

        void IFutureStateMachineConfigurator.SetResult<T>(Event<T> responseReceived,
            Func<BehaviorContext<FutureState, T>, Task> callback)
            where T : class
        {
            SetResult(responseReceived, callback);
        }

        void SetResult<T>(Event<T> resultEvent, Func<BehaviorContext<FutureState, T>, Task> callback)
            where T : class
        {
            DuringAny(
                When(resultEvent)
                    .ThenAsync(context => callback(context))
                    .TransitionTo(Completed)
            );
        }

        public void SetFaulted<T>(Event<T> faultEvent, Func<BehaviorContext<FutureState, T>, Task> callback)
            where T : class
        {
            DuringAny(
                When(faultEvent)
                    .ThenAsync(context => callback(context))
                    .TransitionTo(Faulted)
            );
        }

        static string FormatEventName<T>()
            where T : class
        {
            return DefaultEndpointNameFormatter.Instance.Message<T>();
        }

        protected static Guid RequestIdOrFault(MessageContext context)
        {
            return context.RequestId ?? throw new RequestException("RequestId not present, but required");
        }

        protected static Guid RequestIdOrDefault(MessageContext context)
        {
            return context.RequestId ?? default;
        }

        protected static Guid FutureIdOrFault(ConsumeContext context, IDictionary<string, object> variables)
        {
            if (context.SerializerContext.TryGetValue(variables, MessageHeaders.FutureId, out Guid? correlationId))
                return correlationId.Value;

            throw new RequestException("CorrelationId not present, define the routing slip using Event");
        }

        protected static Guid FutureIdOrDefault(ConsumeContext context, IDictionary<string, object> variables)
        {
            return context.SerializerContext.TryGetValue(variables, MessageHeaders.FutureId, out Guid? correlationId) ? correlationId.Value : default;
        }

        protected void WhenAllCompleted(Action<IFutureResultConfigurator<TResult>> configure)
        {
            var configurator = new FutureResultConfigurator<TCommand, TResult>(_result);

            configure?.Invoke(configurator);
        }

        /// <summary>
        /// When any result faulted, Set the future Faulted
        /// </summary>
        /// <param name="configure"></param>
        protected void WhenAnyFaulted(Action<IFutureFaultConfigurator<TFault>> configure)
        {
            var configurator = new FutureFaultConfigurator<TFault>(_fault);

            configure?.Invoke(configurator);
        }

        static Task<TResult> GetResult(BehaviorContext<FutureState> context)
        {
            if (context.TryGetResult(context.Saga.CorrelationId, out TResult completed))
                return Task.FromResult(completed);

            throw new InvalidOperationException("Completed result not available");
        }

        static Task<TFault> GetFault(BehaviorContext<FutureState> context)
        {
            if (context.TryGetFault(context.Saga.CorrelationId, out TFault faulted))
                return Task.FromResult(faulted);

            throw new InvalidOperationException("Faulted result not available");
        }
    }


    public abstract class Future<TCommand, TResult> :
        Future<TCommand, TResult, Fault<TCommand>>
        where TCommand : class
        where TResult : class
    {
    }
}
