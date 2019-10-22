// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using CorrelationConfigurators;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Internals.Extensions;
    using MassTransit.Scheduling;
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
        readonly Lazy<StateMachineRegistration[]> _registrations;
        Func<TInstance, bool> _isCompleted;

        protected MassTransitStateMachine()
        {
            _registrations = new Lazy<StateMachineRegistration[]>(GetRegistrations);

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
                    EventCorrelation correlation;
                    if (_eventCorrelations.TryGetValue(@event, out correlation))
                        yield return correlation;
                }
            }
        }

        bool SagaStateMachine<TInstance>.IsCompleted(TInstance instance)
        {
            return _isCompleted(instance);
        }

        /// <summary>
        /// Sets the method used to determine if a state machine instance is completed. A completed
        /// state machine instance is removed from the saga repository.
        /// </summary>
        /// <param name="completed"></param>
        protected void SetCompleted(Func<TInstance, bool> completed)
        {
            _isCompleted = completed ?? NotCompletedByDefault;
        }

        /// <summary>
        /// Sets the state machine instance to Completed when in the final state. A completed
        /// state machine instance is removed from the saga repository.
        /// </summary>
        protected void SetCompletedWhenFinalized()
        {
            _isCompleted = IsFinalized;
        }

        bool IsFinalized(TInstance instance)
        {
            State<TInstance> currentState = this.GetState(instance).Result;
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

            EventCorrelation existingCorrelation;
            _eventCorrelations.TryGetValue(@event, out existingCorrelation);

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

            EventCorrelation existingCorrelation;
            _eventCorrelations.TryGetValue(@event, out existingCorrelation);

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

            if (typeof(T).HasInterface<CorrelatedBy<Guid>>())
            {
                var propertyInfo = propertyExpression.GetPropertyInfo();

                var @event = (Event<T>)propertyInfo.GetValue(this);

                var builderType = typeof(CorrelatedByEventCorrelationBuilder<,>).MakeGenericType(typeof(TInstance), typeof(T));
                var builder = (IEventCorrelationBuilder)Activator.CreateInstance(builderType, this, @event);

                _eventCorrelations[@event] = builder.Build();
            }
        }

        void DefaultCorrelatedByConfigurator<T>(IEventCorrelationConfigurator<TInstance, T> configurator)
            where T : class, CorrelatedBy<Guid>
        {
            configurator.CorrelateById(context => context.Message.CorrelationId);
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
            Action<IRequestConfigurator<TInstance, TRequest, TResponse>> configureRequest)
            where TRequest : class
            where TResponse : class
        {
            var configurator = new StateMachineRequestConfigurator<TInstance, TRequest, TResponse>();

            configureRequest(configurator);

            Request(propertyExpression, requestIdExpression, configurator.Settings);
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
            Expression<Func<TInstance, Guid?>> requestIdExpression,
            RequestSettings settings)
            where TRequest : class
            where TResponse : class
        {
            var property = propertyExpression.GetPropertyInfo();

            var requestName = property.Name;

            var request = new StateMachineRequest<TInstance, TRequest, TResponse>(requestName, requestIdExpression, settings);

            property.SetValue(this, request);

            Event(propertyExpression, x => x.Completed, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.Faulted, x => x.CorrelateBy(requestIdExpression, context => context.RequestId));
            Event(propertyExpression, x => x.TimeoutExpired, x => x.CorrelateBy(requestIdExpression, context => context.Message.RequestId));

            State(propertyExpression, x => x.Pending);

            DuringAny(
                When(request.Completed)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.Faulted)
                    .CancelRequestTimeout(request)
                    .ClearRequest(request),
                When(request.TimeoutExpired, request.EventFilter)
                    .ClearRequest(request));
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
            Action<IScheduleConfigurator<TInstance, TMessage>> configureSchedule)
            where TMessage : class
        {
            var configurator = new StateMachineScheduleConfigurator<TInstance, TMessage>();

            configureSchedule(configurator);

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

            property.SetValue(this, schedule);

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
                                if (!tokenId.HasValue || (messageTokenId.Value != tokenId.Value))
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
                            schedule.SetTokenId(context.Instance, default(Guid?));
                    }));
        }

        static bool NotCompletedByDefault(TInstance instance)
        {
            return false;
        }

        /// <summary>
        /// Register all remaining events and states that have not been explicitly declared.
        /// </summary>
        void RegisterImplicit()
        {
            foreach (var declaration in _registrations.Value)
                declaration.Declare(this);
        }

        static IEnumerable<PropertyInfo> GetStateMachineProperties(TypeInfo typeInfo)
        {
            if (typeInfo.IsInterface)
                yield break;

            if (typeInfo.BaseType != null)
            {
                foreach (var propertyInfo in GetStateMachineProperties(typeInfo.BaseType.GetTypeInfo()))
                    yield return propertyInfo;
            }

            IEnumerable<PropertyInfo> properties = typeInfo.DeclaredMethods
                .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && !x.IsStatic)
                .Select(x => typeInfo.GetDeclaredProperty(x.Name.Substring("get_".Length)))
                .Where(x => x.CanRead && x.CanWrite);

            foreach (var propertyInfo in properties)
                yield return propertyInfo;
        }

        StateMachineRegistration[] GetRegistrations()
        {
            var events = new List<StateMachineRegistration>();

            var machineType = GetType();

            IEnumerable<PropertyInfo> properties = GetStateMachineProperties(machineType.GetTypeInfo());

            foreach (var propertyInfo in properties)
            {
                var propertyTypeInfo = propertyInfo.PropertyType.GetTypeInfo();
                if (!propertyTypeInfo.IsGenericType)
                    continue;

                if (propertyTypeInfo.GetGenericTypeDefinition() != typeof(Event<>))
                    continue;

                var messageTypeInfo = propertyTypeInfo.GetGenericArguments().First().GetTypeInfo();
                if (messageTypeInfo.AsType().HasInterface<CorrelatedBy<Guid>>())
                {
                    var declarationType = typeof(CorrelatedEventRegistration<,>).MakeGenericType(typeof(TInstance), machineType,
                        messageTypeInfo.AsType());
                    var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                    events.Add((StateMachineRegistration)declaration);
                }
                else
                {
                    var declarationType = typeof(UncorrelatedEventRegistration<,>).MakeGenericType(typeof(TInstance), machineType,
                        messageTypeInfo.AsType());
                    var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                    events.Add((StateMachineRegistration)declaration);
                }
            }

            return events.ToArray();
        }


        class CorrelatedEventRegistration<TStateMachine, TData> :
            StateMachineRegistration
            where TStateMachine : MassTransitStateMachine<TInstance>
            where TData : class, CorrelatedBy<Guid>
        {
            readonly PropertyInfo _propertyInfo;

            public CorrelatedEventRegistration(PropertyInfo propertyInfo)
            {
                _propertyInfo = propertyInfo;
            }

            public void Declare(object stateMachine)
            {
                var machine = ((TStateMachine)stateMachine);
                var @event = (Event<TData>)_propertyInfo.GetValue(machine);
                if (@event != null)
                {
                    var builderType = typeof(CorrelatedByEventCorrelationBuilder<,>).MakeGenericType(typeof(TInstance), typeof(TData));
                    var builder = (IEventCorrelationBuilder)Activator.CreateInstance(builderType, machine, @event);

                    machine._eventCorrelations[@event] = builder.Build();
                }
            }
        }


        class UncorrelatedEventRegistration<TStateMachine, TData> :
            StateMachineRegistration
            where TStateMachine : MassTransitStateMachine<TInstance>
            where TData : class
        {
            readonly PropertyInfo _propertyInfo;

            public UncorrelatedEventRegistration(PropertyInfo propertyInfo)
            {
                _propertyInfo = propertyInfo;
            }

            public void Declare(object stateMachine)
            {
                var machine = ((TStateMachine)stateMachine);
                var @event = (Event<TData>)_propertyInfo.GetValue(machine);
                if (@event != null)
                {
                    var correlationType = typeof(UncorrelatedEventCorrelation<,>).MakeGenericType(typeof(TInstance), typeof(TData));
                    var correlation = (EventCorrelation<TInstance, TData>)Activator.CreateInstance(correlationType, @event);

                    machine._eventCorrelations[@event] = correlation;
                }
            }
        }


        interface StateMachineRegistration
        {
            void Declare(object stateMachine);
        }
    }
}
