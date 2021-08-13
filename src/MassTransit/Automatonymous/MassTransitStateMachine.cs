namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using CorrelationConfigurators;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Internals.Extensions;
    using MassTransit.Scheduling;
    using MassTransit.Topology.Conventions;
    using MassTransit.Topology.Topologies;
    using MassTransit.Util;
    using Requests;
    using SagaConfigurators;
    using Schedules;


    /// <summary>
    /// A MassTransit state machine adds functionality on top of Automatonymous supporting
    /// things like request/response, and correlating events to the state machine, as well
    /// as retry and policy configuration.
    /// </summary>
    /// <typeparam name="TInstance">The state instance type</typeparam>
    public class MassTransitStateMachine<TInstance> :
        AutomatonymousStateMachine<TInstance>,
        SagaStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly Dictionary<Event, EventCorrelation> _eventCorrelations;
        Func<TInstance, Task<bool>> _isCompleted;

        protected MassTransitStateMachine()
        {
            _eventCorrelations = new Dictionary<Event, EventCorrelation>();
            _isCompleted = NotCompletedByDefault;

            RegisterImplicit();
        }

        public IEnumerable<EventCorrelation> Correlations
        {
            get
            {
                foreach (var @event in Events)
                {
                    if (_eventCorrelations.TryGetValue(@event, out var correlation))
                        yield return correlation;
                }
            }
        }

        Task<bool> SagaStateMachine<TInstance>.IsCompleted(TInstance instance)
        {
            return _isCompleted(instance);
        }

        /// <summary>
        /// Sets the method used to determine if a state machine instance has completed. The saga repository removes completed state machine instances.
        /// </summary>
        /// <param name="completed"></param>
        protected void SetCompleted(Func<TInstance, Task<bool>> completed)
        {
            _isCompleted = completed ?? NotCompletedByDefault;
        }

        /// <summary>
        /// Sets the state machine instance to Completed when in the final state. The saga repository removes completed state machine instances.
        /// </summary>
        protected void SetCompletedWhenFinalized()
        {
            _isCompleted = IsFinalized;
        }

        async Task<bool> IsFinalized(TInstance instance)
        {
            State<TInstance> currentState = await this.GetState(instance).ConfigureAwait(false);

            return Final.Equals(currentState);
        }

        /// <summary>
        /// Declares an Event on the state machine with the specified data type, and allows the correlation of the event
        /// to be configured.
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="propertyExpression">The event property</param>
        /// <param name="configureEventCorrelation">Configuration callback for the event</param>
        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression, Action<IEventCorrelationConfigurator<TInstance, T>> configureEventCorrelation)
            where T : class
        {
            base.Event(propertyExpression);

            var propertyInfo = propertyExpression.GetPropertyInfo();

            var @event = (Event<T>)propertyInfo.GetValue(this);

            _eventCorrelations.TryGetValue(@event, out var existingCorrelation);

            var configurator = new MassTransitEventCorrelationConfigurator<TInstance, T>(this, @event, existingCorrelation);

            configureEventCorrelation(configurator);

            _eventCorrelations[@event] = configurator.Build();
        }

        /// <summary>
        /// Declares an Event on the state machine with the specified data type, and allows the correlation of the event
        /// to be configured.
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="propertyExpression">The containing property</param>
        /// <param name="eventPropertyExpression">The event property expression</param>
        /// <param name="configureEventCorrelation">Configuration callback for the event</param>
        protected void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, Event<T>>> eventPropertyExpression,
            Action<IEventCorrelationConfigurator<TInstance, T>> configureEventCorrelation)
            where TProperty : class
            where T : class
        {
            base.Event(propertyExpression, eventPropertyExpression);

            var propertyInfo = propertyExpression.GetPropertyInfo();
            var property = (TProperty)propertyInfo.GetValue(this);

            var eventPropertyInfo = eventPropertyExpression.GetPropertyInfo();
            var @event = (Event<T>)eventPropertyInfo.GetValue(property);

            _eventCorrelations.TryGetValue(@event, out var existingCorrelation);

            var configurator = new MassTransitEventCorrelationConfigurator<TInstance, T>(this, @event, existingCorrelation);

            configureEventCorrelation(configurator);

            _eventCorrelations[@event] = configurator.Build();
        }

        /// <summary>
        /// Declares an event on the state machine with the specified data type, where the data type contains the
        /// CorrelatedBy(Guid) interface. The correlation by CorrelationId is automatically configured to the saga
        /// instance.
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="propertyExpression">The property to initialize</param>
        protected override void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            base.Event(propertyExpression);

            var propertyInfo = propertyExpression.GetPropertyInfo();

            var @event = (Event)propertyInfo.GetValue(this);

            var registration = GetEventRegistration(@event, typeof(T));

            registration.RegisterCorrelation(this);
        }

        /// <summary>
        /// Declares an Event on the state machine with the specified data type, and allows the correlation of the event
        /// to be configured.
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="name">The event name (must be unique)</param>
        protected override Event<T> Event<T>(string name)
        {
            Event<T> @event = base.Event<T>(name);

            var registration = GetEventRegistration(@event, typeof(T));

            registration.RegisterCorrelation(this);

            return @event;
        }

        /// <summary>
        /// Declares an Event on the state machine with the specified data type, and allows the correlation of the event
        /// to be configured.
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="name">The event name (must be unique)</param>
        /// <param name="configure">Configuration callback method</param>
        protected Event<T> Event<T>(string name, Action<IEventCorrelationConfigurator<TInstance, T>> configure)
            where T : class
        {
            Event<T> @event = Event<T>(name);

            _eventCorrelations.TryGetValue(@event, out var existingCorrelation);

            var configurator = new MassTransitEventCorrelationConfigurator<TInstance, T>(this, @event, existingCorrelation);

            configure?.Invoke(configurator);

            _eventCorrelations[@event] = configurator.Build();

            return @event;
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="requestIdExpression">The property where the requestId is stored</param>
        /// <param name="configureRequest">Allow the request settings to be specified inline</param>
        protected void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> requestIdExpression,
            Action<IRequestConfigurator> configureRequest = default)
            where TRequest : class
            where TResponse : class
        {
            var configurator = new StateMachineRequestConfigurator<TRequest>();

            configureRequest?.Invoke(configurator);

            Request(propertyExpression, requestIdExpression, configurator.Settings);
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// Uses the Saga CorrelationId as the RequestId
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="configureRequest">Allow the request settings to be specified inline</param>
        protected void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression,
            Action<IRequestConfigurator> configureRequest = default)
            where TRequest : class
            where TResponse : class
        {
            var configurator = new StateMachineRequestConfigurator<TRequest>();

            configureRequest?.Invoke(configurator);

            Request(propertyExpression, configurator.Settings);
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="requestIdExpression">The property where the requestId is stored</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.)</param>
        protected void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> requestIdExpression, RequestSettings settings)
            where TRequest : class
            where TResponse : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var request = new StateMachineRequest<TInstance, TRequest, TResponse>(property.Name, settings, requestIdExpression);

            InitializeRequest(this, property, request);

            Event(propertyExpression, x => x.Completed, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.Faulted, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.TimeoutExpired, x => x.CorrelateBy(requestIdExpression, context => context.Message.RequestId));

            State(propertyExpression, x => x.Pending);

            DuringAny(
                When(request.Completed)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.Faulted)
                    .CancelRequestTimeout(request));
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// Uses the Saga CorrelationId as the RequestId
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.)</param>
        protected void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression,
            RequestSettings settings)
            where TRequest : class
            where TResponse : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var request = new StateMachineRequest<TInstance, TRequest, TResponse>(property.Name, settings);

            InitializeRequest(this, property, request);

            Event(propertyExpression, x => x.Completed, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.Faulted, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.TimeoutExpired, x => x.CorrelateById(context => context.Message.RequestId));

            State(propertyExpression, x => x.Pending);

            DuringAny(
                When(request.Completed)
                    .CancelRequestTimeout(request),
                When(request.Faulted)
                    .CancelRequestTimeout(request));
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="requestIdExpression">The property where the requestId is stored</param>
        /// <param name="configureRequest">Allow the request settings to be specified inline</param>
        protected void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> requestIdExpression,
            Action<IRequestConfigurator> configureRequest = default)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
        {
            var configurator = new StateMachineRequestConfigurator<TRequest>();

            configureRequest?.Invoke(configurator);

            Request(propertyExpression, requestIdExpression, configurator.Settings);
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// Uses the Saga CorrelationId as the RequestId
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="configureRequest">Allow the request settings to be specified inline</param>
        protected void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression,
            Action<IRequestConfigurator> configureRequest = default)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
        {
            var configurator = new StateMachineRequestConfigurator<TRequest>();

            configureRequest?.Invoke(configurator);

            Request(propertyExpression, configurator.Settings);
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="requestIdExpression">The property where the requestId is stored</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.)</param>
        protected void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> requestIdExpression, RequestSettings settings)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var request = new StateMachineRequest<TInstance, TRequest, TResponse, TResponse2>(property.Name, settings, requestIdExpression);

            InitializeRequest(this, property, request);

            Event(propertyExpression, x => x.Completed, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.Completed2, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.Faulted, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.TimeoutExpired, x => x.CorrelateBy(requestIdExpression, context => context.Message.RequestId));

            State(propertyExpression, x => x.Pending);

            DuringAny(
                When(request.Completed)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.Completed2)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.Faulted)
                    .CancelRequestTimeout(request));
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// Uses the Saga CorrelationId as the RequestId
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.)</param>
        protected void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression,
            RequestSettings settings)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var request = new StateMachineRequest<TInstance, TRequest, TResponse, TResponse2>(property.Name, settings);

            InitializeRequest(this, property, request);

            Event(propertyExpression, x => x.Completed, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.Completed2, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.Faulted, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.TimeoutExpired, x => x.CorrelateById(context => context.Message.RequestId));

            State(propertyExpression, x => x.Pending);

            DuringAny(
                When(request.Completed)
                    .CancelRequestTimeout(request),
                When(request.Completed2)
                    .CancelRequestTimeout(request),
                When(request.Faulted)
                    .CancelRequestTimeout(request));
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <typeparam name="TResponse3"></typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="requestIdExpression">The property where the requestId is stored</param>
        /// <param name="configureRequest">Allow the request settings to be specified inline</param>
        protected void Request<TRequest, TResponse, TResponse2, TResponse3>(
            Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> requestIdExpression,
            Action<IRequestConfigurator> configureRequest = default)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
            where TResponse3 : class
        {
            var configurator = new StateMachineRequestConfigurator<TRequest>();

            configureRequest?.Invoke(configurator);

            Request(propertyExpression, requestIdExpression, configurator.Settings);
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// Uses the Saga CorrelationId as the RequestId
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <typeparam name="TResponse3"></typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="configureRequest">Allow the request settings to be specified inline</param>
        protected void Request<TRequest, TResponse, TResponse2, TResponse3>(
            Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression,
            Action<IRequestConfigurator> configureRequest = default)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
            where TResponse3 : class
        {
            var configurator = new StateMachineRequestConfigurator<TRequest>();

            configureRequest?.Invoke(configurator);

            Request(propertyExpression, configurator.Settings);
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <typeparam name="TResponse3"></typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="requestIdExpression">The property where the requestId is stored</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.)</param>
        protected void Request<TRequest, TResponse, TResponse2, TResponse3>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> requestIdExpression, RequestSettings settings)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
            where TResponse3 : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var request = new StateMachineRequest<TInstance, TRequest, TResponse, TResponse2, TResponse3>(property.Name, settings, requestIdExpression);

            InitializeRequest(this, property, request);

            Event(propertyExpression, x => x.Completed, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.Completed2, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.Completed3, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.Faulted, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.TimeoutExpired, x => x.CorrelateBy(requestIdExpression, context => context.Message.RequestId));

            State(propertyExpression, x => x.Pending);

            DuringAny(
                When(request.Completed)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.Completed2)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.Completed3)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.Faulted)
                    .CancelRequestTimeout(request));
        }

        /// <summary>
        /// Declares a request that is sent by the state machine to a service, and the associated response, fault, and
        /// timeout handling. The property is initialized with the fully built Request. The request must be declared before
        /// it is used in the state/event declaration statements.
        /// Uses the Saga CorrelationId as the RequestId
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <typeparam name="TResponse2">The alternate response type</typeparam>
        /// <typeparam name="TResponse3"></typeparam>
        /// <param name="propertyExpression">The request property on the state machine</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.)</param>
        protected void Request<TRequest, TResponse, TResponse2, TResponse3>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression,
            RequestSettings settings)
            where TRequest : class
            where TResponse : class
            where TResponse2 : class
            where TResponse3 : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var request = new StateMachineRequest<TInstance, TRequest, TResponse, TResponse2, TResponse3>(property.Name, settings);

            InitializeRequest(this, property, request);

            Event(propertyExpression, x => x.Completed, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.Completed2, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.Completed3, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.Faulted, x => x.CorrelateById(context => context.RequestId ?? throw new RequestException("Missing RequestId")));
            Event(propertyExpression, x => x.TimeoutExpired, x => x.CorrelateById(context => context.Message.RequestId));

            State(propertyExpression, x => x.Pending);

            DuringAny(
                When(request.Completed)
                    .CancelRequestTimeout(request),
                When(request.Completed2)
                    .CancelRequestTimeout(request),
                When(request.Completed3)
                    .CancelRequestTimeout(request),
                When(request.Faulted)
                    .CancelRequestTimeout(request));
        }

        /// <summary>
        /// Declares a schedule placeholder that is stored with the state machine instance
        /// </summary>
        /// <typeparam name="TMessage">The request type</typeparam>
        /// <param name="propertyExpression">The schedule property on the state machine</param>
        /// <param name="tokenIdExpression">The property where the tokenId is stored</param>
        /// <param name="configureSchedule">The callback to configure the schedule</param>
        protected void Schedule<TMessage>(Expression<Func<Schedule<TInstance, TMessage>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> tokenIdExpression,
            Action<IScheduleConfigurator<TInstance, TMessage>> configureSchedule = default)
            where TMessage : class
        {
            var configurator = new StateMachineScheduleConfigurator<TInstance, TMessage>();

            configureSchedule?.Invoke(configurator);

            Schedule(propertyExpression, tokenIdExpression, configurator.Settings);
        }

        /// <summary>
        /// Declares a schedule placeholder that is stored with the state machine instance
        /// </summary>
        /// <typeparam name="TMessage">The scheduled message type</typeparam>
        /// <param name="propertyExpression">The schedule property on the state machine</param>
        /// <param name="tokenIdExpression">The property where the tokenId is stored</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.)</param>
        protected void Schedule<TMessage>(Expression<Func<Schedule<TInstance, TMessage>>> propertyExpression,
            Expression<Func<TInstance, Guid?>> tokenIdExpression,
            ScheduleSettings<TInstance, TMessage> settings)
            where TMessage : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var name = property.Name;

            var schedule = new StateMachineSchedule<TInstance, TMessage>(name, tokenIdExpression, settings);

            InitializeSchedule(this, property, schedule);

            Event(propertyExpression, x => x.Received);

            if (settings.Received == null)
                Event(propertyExpression, x => x.AnyReceived);
            else
            {
                Event(propertyExpression, x => x.AnyReceived, x =>
                {
                    settings.Received(x);
                });
            }

            DuringAny(
                When(schedule.AnyReceived)
                    .ThenAsync(async context =>
                    {
                        Guid? tokenId = schedule.GetTokenId(context.Instance);

                        if (context.TryGetPayload(out ConsumeContext consumeContext))
                        {
                            Guid? messageTokenId = consumeContext.GetSchedulingTokenId();
                            if (messageTokenId.HasValue)
                            {
                                if (!tokenId.HasValue || messageTokenId.Value != tokenId.Value)
                                {
                                    LogContext.Debug?.Log("SAGA: {CorrelationId} Scheduled message not current: {TokenId}", context.Instance.CorrelationId,
                                        messageTokenId.Value);

                                    return;
                                }
                            }
                        }

                        BehaviorContext<TInstance, TMessage> eventContext = context.GetProxy(schedule.Received, context.Data);

                        await ((StateMachine<TInstance>)this).RaiseEvent(eventContext).ConfigureAwait(false);

                        if (schedule.GetTokenId(context.Instance) == tokenId)
                            schedule.SetTokenId(context.Instance, default);
                    }));
        }

        static Task<bool> NotCompletedByDefault(TInstance instance)
        {
            return TaskUtil.False;
        }

        static void InitializeSchedule<T>(AutomatonymousStateMachine<TInstance> stateMachine, PropertyInfo property, Schedule<TInstance, T> schedule)
            where T : class
        {
            if (property.CanWrite)
                property.SetValue(stateMachine, schedule);
            else if (ConfigurationHelpers.TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                backingField.SetValue(stateMachine, schedule);
            else
                throw new ArgumentException($"The schedule property is not writable: {property.Name}");
        }

        static void InitializeRequest<TRequest, TResponse>(AutomatonymousStateMachine<TInstance> stateMachine, PropertyInfo property,
            Request<TInstance, TRequest, TResponse> request)
            where TRequest : class
            where TResponse : class
        {
            if (property.CanWrite)
                property.SetValue(stateMachine, request);
            else if (ConfigurationHelpers.TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                backingField.SetValue(stateMachine, request);
            else
                throw new ArgumentException($"The request property is not writable: {property.Name}");
        }

        /// <summary>
        /// Register all remaining events and states that have not been explicitly declared.
        /// </summary>
        void RegisterImplicit()
        {
            var machineType = GetType().GetTypeInfo();

            IEnumerable<PropertyInfo> properties = ConfigurationHelpers.GetStateMachineProperties(machineType);

            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType.GetTypeInfo();
                if (!propertyType.IsGenericType)
                    continue;

                if (!propertyType.ClosesType(typeof(Event<>), out Type[] arguments))
                    continue;

                var @event = (Event)propertyInfo.GetValue(this);
                if (@event == null)
                    continue;

                var registration = GetEventRegistration(@event, arguments[0]);
                registration.RegisterCorrelation(this);
            }
        }

        static EventRegistration GetEventRegistration(Event @event, Type messageType)
        {
            if (messageType.HasInterface<CorrelatedBy<Guid>>())
            {
                var registrationType = typeof(CorrelatedEventRegistration<>).MakeGenericType(typeof(TInstance), messageType);
                return (EventRegistration)Activator.CreateInstance(registrationType, @event);
            }
            else
            {
                var registrationType = typeof(UncorrelatedEventRegistration<>).MakeGenericType(typeof(TInstance), messageType);
                return (EventRegistration)Activator.CreateInstance(registrationType, @event);
            }
        }


        class CorrelatedEventRegistration<TData> :
            EventRegistration
            where TData : class, CorrelatedBy<Guid>
        {
            readonly Event<TData> _event;

            public CorrelatedEventRegistration(Event<TData> @event)
            {
                _event = @event;
            }

            public void RegisterCorrelation(MassTransitStateMachine<TInstance> machine)
            {
                var builderType = typeof(CorrelatedByEventCorrelationBuilder<,>).MakeGenericType(typeof(TInstance), typeof(TData));
                var builder = (IEventCorrelationBuilder)Activator.CreateInstance(builderType, machine, _event);

                machine._eventCorrelations[_event] = builder.Build();
            }
        }


        class UncorrelatedEventRegistration<TData> :
            EventRegistration
            where TData : class
        {
            readonly Event<TData> _event;

            public UncorrelatedEventRegistration(Event<TData> @event)
            {
                _event = @event;
            }

            public void RegisterCorrelation(MassTransitStateMachine<TInstance> machine)
            {
                if (GlobalTopology.Send.GetMessageTopology<TData>().TryGetConvention(out ICorrelationIdMessageSendTopologyConvention<TData> convention)
                    && convention.TryGetMessageCorrelationId(out IMessageCorrelationId<TData> messageCorrelationId))
                {
                    var builderType = typeof(MessageCorrelationIdEventCorrelationBuilder<,>).MakeGenericType(typeof(TInstance), typeof(TData));
                    var builder = (IEventCorrelationBuilder)Activator.CreateInstance(builderType, machine, _event, messageCorrelationId);

                    machine._eventCorrelations[_event] = builder.Build();
                }
                else
                {
                    var correlationType = typeof(UncorrelatedEventCorrelation<,>).MakeGenericType(typeof(TInstance), typeof(TData));
                    var correlation = (EventCorrelation<TInstance, TData>)Activator.CreateInstance(correlationType, _event);

                    machine._eventCorrelations[_event] = correlation;
                }
            }
        }


        interface EventRegistration
        {
            void RegisterCorrelation(MassTransitStateMachine<TInstance> machine);
        }
    }
}
