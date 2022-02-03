namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Configuration;
    using Internals;
    using SagaStateMachine;
    using Util;


    /// <summary>
    /// A MassTransit state machine adds functionality on top of Automatonymous supporting
    /// things like request/response, and correlating events to the state machine, as well
    /// as retry and policy configuration.
    /// </summary>
    /// <typeparam name="TInstance">The state instance type</typeparam>
    public class MassTransitStateMachine<TInstance> :
        SagaStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly Dictionary<string, Dictionary<string, List<EventActivityBinder<TInstance>>>> _compositeBindings;
        readonly HashSet<string> _compositeEvents;
        readonly Dictionary<string, StateMachineEvent<TInstance>> _eventCache;
        readonly Dictionary<Event, EventCorrelation> _eventCorrelations;
        readonly EventObservable<TInstance> _eventObservers;
        readonly State<TInstance> _final;
        readonly State<TInstance> _initial;
        readonly Lazy<ConfigurationHelpers.StateMachineRegistration[]> _registrations;
        readonly Dictionary<string, State<TInstance>> _stateCache;
        readonly StateObservable<TInstance> _stateObservers;
        IStateAccessor<TInstance> _accessor;
        Func<BehaviorContext<TInstance>, Task<bool>> _isCompleted;
        string _name;
        UnhandledEventCallback<TInstance> _unhandledEventCallback;

        protected MassTransitStateMachine()
        {
            _registrations = new Lazy<ConfigurationHelpers.StateMachineRegistration[]>(() => ConfigurationHelpers.GetRegistrations(this));
            _stateCache = new Dictionary<string, State<TInstance>>();
            _eventCache = new Dictionary<string, StateMachineEvent<TInstance>>();
            _compositeBindings = new Dictionary<string, Dictionary<string, List<EventActivityBinder<TInstance>>>>();
            _compositeEvents = new HashSet<string>();

            _eventObservers = new EventObservable<TInstance>();
            _stateObservers = new StateObservable<TInstance>();

            _initial = new StateMachineState<TInstance>((context, state) => UnhandledEvent(context, state), "Initial", _eventObservers);
            _stateCache[_initial.Name] = _initial;
            _final = new StateMachineState<TInstance>((context, state) => UnhandledEvent(context, state), "Final", _eventObservers);
            _stateCache[_final.Name] = _final;

            _accessor = new DefaultInstanceStateAccessor<TInstance>(this, _stateCache[Initial.Name], _stateObservers);

            _unhandledEventCallback = DefaultUnhandledEventCallback;

            _name = GetType().Name;

            _eventCorrelations = new Dictionary<Event, EventCorrelation>();
            _isCompleted = NotCompletedByDefault;

            RegisterImplicit();
        }

        IEnumerable<State<TInstance>> IntrospectionStates
        {
            get
            {
                yield return _initial;

                foreach (State<TInstance> x in _stateCache.Values)
                {
                    if (Equals(x, Initial) || Equals(x, Final))
                        continue;

                    yield return x;
                }

                yield return _final;
            }
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

        Task<bool> SagaStateMachine<TInstance>.IsCompleted(BehaviorContext<TInstance> context)
        {
            return _isCompleted(context);
        }

        string StateMachine.Name => _name;
        public IStateAccessor<TInstance> Accessor => _accessor;
        public State Initial => _initial;
        public State Final => _final;

        State StateMachine.GetState(string name)
        {
            if (_stateCache.TryGetValue(name, out State<TInstance> result))
                return result;

            throw new UnknownStateException(_name, name);
        }

        async Task StateMachine<TInstance>.RaiseEvent(BehaviorContext<TInstance> context)
        {
            State<TInstance> state = await _accessor.Get(context).ConfigureAwait(false);

            if (!_stateCache.TryGetValue(state.Name, out State<TInstance> instanceState))
                throw new UnknownStateException(_name, state.Name);

            await instanceState.Raise(context).ConfigureAwait(false);
        }

        async Task StateMachine<TInstance>.RaiseEvent<T>(BehaviorContext<TInstance, T> context)
        {
            State<TInstance> state = await _accessor.Get(context).ConfigureAwait(false);

            if (!_stateCache.TryGetValue(state.Name, out State<TInstance> instanceState))
                throw new UnknownStateException(_name, state.Name);

            await instanceState.Raise(context).ConfigureAwait(false);
        }

        public State<TInstance> GetState(string name)
        {
            if (TryGetState(name, out State<TInstance> result))
                return result;

            throw new UnknownStateException(_name, name);
        }

        public IEnumerable<State> States => _stateCache.Values;

        Event StateMachine.GetEvent(string name)
        {
            if (_eventCache.TryGetValue(name, out StateMachineEvent<TInstance> result))
                return result.Event;

            throw new UnknownEventException(_name, name);
        }

        public IEnumerable<Event> Events
        {
            get { return _eventCache.Values.Where(x => false == x.IsTransitionEvent).Select(x => x.Event); }
        }

        Type StateMachine.InstanceType => typeof(TInstance);

        public IEnumerable<Event> NextEvents(State state)
        {
            if (_stateCache.TryGetValue(state.Name, out State<TInstance> result))
                return result.Events;

            throw new UnknownStateException(_name, state.Name);
        }

        public bool IsCompositeEvent(Event @event)
        {
            return _compositeEvents.Contains(@event.Name);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            foreach (State<TInstance> x in IntrospectionStates)
                x.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("stateMachine");

            var stateMachineType = GetType();
            scope.Add("name", stateMachineType.Name);
            scope.Add("instanceType", TypeCache<TInstance>.ShortName);

            _accessor.Probe(scope);

            foreach (State<TInstance> state in IntrospectionStates)
                state.Probe(scope);
        }

        public IDisposable ConnectEventObserver(IEventObserver<TInstance> observer)
        {
            var eventObserver = new NonTransitionEventObserver<TInstance>(_eventCache, observer);

            return _eventObservers.Connect(eventObserver);
        }

        public IDisposable ConnectEventObserver(Event @event, IEventObserver<TInstance> observer)
        {
            var eventObserver = new SelectedEventObserver<TInstance>(@event, observer);

            return _eventObservers.Connect(eventObserver);
        }

        public IDisposable ConnectStateObserver(IStateObserver<TInstance> stateObserver)
        {
            return _stateObservers.Connect(stateObserver);
        }

        bool TryGetState(string name, out State<TInstance> state)
        {
            return _stateCache.TryGetValue(name, out state);
        }

        Task DefaultUnhandledEventCallback(UnhandledEventContext<TInstance> context)
        {
            throw new UnhandledEventException(_name, context.Event.Name, context.CurrentState.Name);
        }

        /// <summary>
        /// Declares what property holds the TInstance's state on the current instance of the state machine
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <remarks>
        /// Setting the state accessor more than once will cause the property managed by the state machine to change each time.
        /// Please note, the state machine can only manage one property at a given time per instance,
        /// and the best practice is to manage one property per machine.
        /// </remarks>
        protected internal void InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
        {
            var stateAccessor = new RawStateAccessor<TInstance>(this, instanceStateProperty, _stateObservers);

            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Declares the property to hold the instance's state as a string (the state name is stored in the property)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        protected internal void InstanceState(Expression<Func<TInstance, string>> instanceStateProperty)
        {
            var stateAccessor = new StringStateAccessor<TInstance>(this, instanceStateProperty, _stateObservers);

            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Declares the property to hold the instance's state as an int (0 - none, 1 = initial, 2 = final, 3... the rest)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <param name="states">Specifies the states, in order, to which the int values should be assigned</param>
        protected internal void InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states)
        {
            var stateIndex = new StateAccessorIndex<TInstance>(this, _initial, _final, states);

            var stateAccessor = new IntStateAccessor<TInstance>(instanceStateProperty, stateIndex, _stateObservers);

            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Specifies the name of the state machine
        /// </summary>
        /// <param name="machineName"></param>
        protected internal void Name(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                throw new ArgumentException("The machine name must not be empty", nameof(machineName));

            _name = machineName;
        }

        /// <summary>
        /// Declares an event, and initializes the event property
        /// </summary>
        /// <param name="propertyExpression"></param>
        protected internal void Event(Expression<Func<Event>> propertyExpression)
        {
            DeclarePropertyBasedEvent(prop => DeclareTriggerEvent(prop.Name), propertyExpression.GetPropertyInfo());
        }

        protected internal Event Event(string name)
        {
            return DeclareTriggerEvent(name);
        }

        Event DeclareTriggerEvent(string name)
        {
            return DeclareEvent(_ => new TriggerEvent(name), name);
        }

        /// <summary>
        /// Sets the method used to determine if a state machine instance has completed. The saga repository removes completed state machine instances.
        /// </summary>
        /// <param name="completed"></param>
        protected void SetCompleted(Func<TInstance, Task<bool>> completed)
        {
            _isCompleted = completed != null
                ? (Func<BehaviorContext<TInstance>, Task<bool>>)(context => completed(context.Saga))
                : NotCompletedByDefault;
        }

        protected void SetCompleted(Func<BehaviorContext<TInstance>, Task<bool>> completed)
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

        async Task<bool> IsFinalized(BehaviorContext<TInstance> context)
        {
            State<TInstance> currentState = await Accessor.Get(context).ConfigureAwait(false);

            return Final.Equals(currentState);
        }

        Event<T> DeclareDataEvent<T>(string name)
            where T : class
        {
            return DeclareEvent(_ => new MessageEvent<T>(name), name);
        }

        void DeclarePropertyBasedEvent<TEvent>(Func<PropertyInfo, TEvent> ctor, PropertyInfo property)
            where TEvent : Event
        {
            var @event = ctor(property);
            ConfigurationHelpers.InitializeEvent(this, property, @event);
        }

        TEvent DeclareEvent<TEvent>(Func<string, TEvent> ctor, string name)
            where TEvent : Event
        {
            var @event = ctor(name);
            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);
            return @event;
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
            Event(propertyExpression);

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
        protected internal void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, Event<T>>> eventPropertyExpression,
            Action<IEventCorrelationConfigurator<TInstance, T>> configureEventCorrelation)
            where TProperty : class
            where T : class
        {
            Event(propertyExpression, eventPropertyExpression);

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
        /// Declares a data event on a property of the state machine, and initializes the property
        /// </summary>
        /// <param name="propertyExpression">The property</param>
        /// <param name="eventPropertyExpression">The event property on the property</param>
        protected internal void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
            where TProperty : class
            where T : class
        {
            var property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var eventProperty = eventPropertyExpression.GetPropertyInfo();

            var name = $"{property.Name}.{eventProperty.Name}";

            var @event = new MessageEvent<T>(name);

            ConfigurationHelpers.InitializeEventProperty<TProperty, T>(eventProperty, propertyValue, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);
        }

        /// <summary>
        /// Declares an event on the state machine with the specified data type, where the data type contains the
        /// CorrelatedBy(Guid) interface. The correlation by CorrelationId is automatically configured to the saga
        /// instance.
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="propertyExpression">The property to initialize</param>
        protected internal void Event<T>(Expression<Func<Event<T>>> propertyExpression)
            where T : class
        {
            DeclarePropertyBasedEvent(prop => DeclareDataEvent<T>(prop.Name), propertyExpression.GetPropertyInfo());

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
        protected internal Event<T> Event<T>(string name)
            where T : class
        {
            Event<T> @event = DeclareDataEvent<T>(name);

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
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events)
        {
            return CompositeEvent(propertyExpression, trackingPropertyExpression, CompositeEventOptions.None, events);
        }

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="options">Options on the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options, params Event[] events)
        {
            var trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            var accessor = new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyInfo);

            return CompositeEvent(propertyExpression, accessor, options, events);
        }

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events)
        {
            return CompositeEvent(propertyExpression, trackingPropertyExpression, CompositeEventOptions.None, events);
        }

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="options">Options on the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options, params Event[] events)
        {
            var trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            var accessor = new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyInfo);

            return CompositeEvent(propertyExpression, accessor, options, events);
        }

        internal Event CompositeEvent(string name, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, params Event[] events)
        {
            return CompositeEvent(name, trackingPropertyExpression, CompositeEventOptions.None, events);
        }

        protected internal Event CompositeEvent(string name, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(name, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), options, events);
        }

        internal Event CompositeEvent(string name, Expression<Func<TInstance, int>> trackingPropertyExpression, params Event[] events)
        {
            return CompositeEvent(name, trackingPropertyExpression, CompositeEventOptions.None, events);
        }

        protected internal Event CompositeEvent(string name, Expression<Func<TInstance, int>> trackingPropertyExpression, CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(name, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), options, events);
        }

        protected internal Event CompositeEvent(Event @event, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events)
        {
            return CompositeEvent(@event, trackingPropertyExpression, CompositeEventOptions.None, events);
        }

        protected internal Event CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(@event, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), options, events);
        }

        protected internal Event CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events)
        {
            return CompositeEvent(@event, trackingPropertyExpression, CompositeEventOptions.None, events);
        }

        protected internal Event CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(@event, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                options, events);
        }

        Event CompositeEvent(Expression<Func<Event>> propertyExpression, ICompositeEventStatusAccessor<TInstance> accessor,
            CompositeEventOptions options, Event[] events)
        {
            Event CreateEvent()
            {
                var eventProperty = propertyExpression.GetPropertyInfo();

                var @event = new TriggerEvent(eventProperty.Name);

                ConfigurationHelpers.InitializeEvent(this, eventProperty, @event);

                _eventCache[eventProperty.Name] = new StateMachineEvent<TInstance>(@event, false);

                return @event;
            }

            return CompositeEvent(CreateEvent(), accessor, options, events);
        }

        Event CompositeEvent(string name, ICompositeEventStatusAccessor<TInstance> accessor, CompositeEventOptions options, Event[] events)
        {
            Event CreateEvent()
            {
                var @event = new TriggerEvent(name);

                _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);

                return @event;
            }

            return CompositeEvent(CreateEvent(), accessor, options, events);
        }

        Event CompositeEvent(Event @event, ICompositeEventStatusAccessor<TInstance> accessor, CompositeEventOptions options, Event[] events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            if (events.Length > 31)
                throw new ArgumentException("No more than 31 events can be combined into a single event");
            if (events.Length == 0)
                throw new ArgumentException("At least one event must be specified for a composite event");
            if (events.Any(x => x == null))
                throw new ArgumentException("One or more events specified has not yet been initialized");

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length).Aggregate(0, (current, x) => current | (1 << x)));

            _compositeEvents.Add(@event.Name);

            for (var i = 0; i < events.Length; i++)
            {
                var flag = 1 << i;

                var activity = new CompositeEventActivity<TInstance>(accessor, flag, complete, @event);

                bool Filter(State<TInstance> state)
                {
                    if (Equals(state, Initial))
                        return options.HasFlag(CompositeEventOptions.IncludeInitial);

                    if (Equals(state, Final))
                        return options.HasFlag(CompositeEventOptions.IncludeFinal);

                    return true;
                }

                List<State<TInstance>> states = _stateCache.Values.Where(Filter).ToList();

                foreach (State<TInstance> state in states)
                {
                    // Set the IsComposite flag just to make sure it is really set.
                    var currentEvent = state.Events.FirstOrDefault(x => Equals(x, @event));
                    if (currentEvent != null)
                        _compositeEvents.Add(currentEvent.Name);

                    // Determine which event the composited event belongs to
                    State<TInstance> boundToState = _stateCache.Values.FirstOrDefault(s => s.Events.Any(evt => evt.Name == events[i].Name));
                    State<TInstance> bindingState = boundToState ?? state;

                    if (!_compositeBindings.ContainsKey(@event.Name))
                        _compositeBindings[@event.Name] = new Dictionary<string, List<EventActivityBinder<TInstance>>>();

                    if (!_compositeBindings[@event.Name].ContainsKey(bindingState.Name))
                        _compositeBindings[@event.Name][bindingState.Name] = new List<EventActivityBinder<TInstance>>();

                    if (_compositeBindings[@event.Name][bindingState.Name].All(x => x.Event.Name != events[i].Name))
                        _compositeBindings[@event.Name][bindingState.Name].Add(When(events[i]).Execute(activity));
                }
            }

            return @event;
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The state property</param>
        protected internal void State(Expression<Func<State>> propertyExpression)
        {
            var property = propertyExpression.GetPropertyInfo();

            DeclareState(property);
        }

        protected internal State<TInstance> State(string name)
        {
            if (TryGetState(name, out State<TInstance> foundState))
                return foundState;

            var state = new StateMachineState<TInstance>((c, s) => UnhandledEvent(c, s), name, _eventObservers);
            SetState(name, state);

            return state;
        }

        void DeclareState(PropertyInfo property)
        {
            var name = property.Name;

            var propertyValue = property.GetValue(this);

            // If the state was already defined, don't define it again
            var existingState = propertyValue as StateMachineState<TInstance>;
            if (name.Equals(existingState?.Name))
                return;

            var state = new StateMachineState<TInstance>((c, s) => UnhandledEvent(c, s), name, _eventObservers);

            ConfigurationHelpers.InitializeState(this, property, state);

            SetState(name, state);
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The property containing the state</param>
        /// <param name="statePropertyExpression">The state property</param>
        protected internal void State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression)
            where TProperty : class
        {
            var property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var stateProperty = statePropertyExpression.GetPropertyInfo();

            var name = $"{property.Name}.{stateProperty.Name}";

            StateMachineState<TInstance> existingState = GetStateProperty(stateProperty, propertyValue);
            if (name.Equals(existingState?.Name))
                return;

            var state = new StateMachineState<TInstance>((c, s) => UnhandledEvent(c, s), name, _eventObservers);

            ConfigurationHelpers.InitializeStateProperty(stateProperty, propertyValue, state);

            SetState(name, state);
        }

        static StateMachineState<TInstance> GetStateProperty<TProperty>(PropertyInfo stateProperty, TProperty propertyValue)
            where TProperty : class
        {
            if (stateProperty.CanRead)
                return stateProperty.GetValue(propertyValue) as StateMachineState<TInstance>;

            var objectProperty = propertyValue.GetType().GetProperty(stateProperty.Name, typeof(State));
            if (objectProperty == null || !objectProperty.CanRead)
                throw new ArgumentException($"The state property is not readable: {stateProperty.Name}");

            return objectProperty.GetValue(propertyValue) as StateMachineState<TInstance>;
        }

        /// <summary>
        /// Declares a sub-state on the machine. A sub-state is a state that is valid within a super-state,
        /// allowing a state machine to have multiple "states" -- nested parts of an overall state.
        /// </summary>
        /// <param name="propertyExpression">The state property expression</param>
        /// <param name="superState">The superstate of which this state is a substate</param>
        protected internal void SubState(Expression<Func<State>> propertyExpression, State superState)
        {
            if (superState == null)
                throw new ArgumentNullException(nameof(superState));

            State<TInstance> superStateInstance = GetState(superState.Name);

            var property = propertyExpression.GetPropertyInfo();

            var name = property.Name;

            var propertyValue = property.GetValue(this);

            // If the state was already defined, don't define it again
            var existingState = propertyValue as StateMachineState<TInstance>;
            if (name.Equals(existingState?.Name) && superState.Name.Equals(existingState?.SuperState?.Name))
                return;

            var state = new StateMachineState<TInstance>((c, s) => UnhandledEvent(c, s), name, _eventObservers, superStateInstance);

            ConfigurationHelpers.InitializeState(this, property, state);

            SetState(name, state);
        }

        protected internal State<TInstance> SubState(string name, State superState)
        {
            if (superState == null)
                throw new ArgumentNullException(nameof(superState));

            State<TInstance> superStateInstance = GetState(superState.Name);

            // If the state was already defined, don't define it again
            if (TryGetState(name, out State<TInstance> existingState) &&
                name.Equals(existingState?.Name) &&
                superState.Name.Equals(existingState?.SuperState?.Name))
                return existingState;

            var state = new StateMachineState<TInstance>((c, s) => UnhandledEvent(c, s), name, _eventObservers, superStateInstance);

            SetState(name, state);
            return state;
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The property containing the state</param>
        /// <param name="statePropertyExpression">The state property</param>
        /// <param name="superState">The superstate of which this state is a substate</param>
        protected internal void SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState)
            where TProperty : class
        {
            if (superState == null)
                throw new ArgumentNullException(nameof(superState));

            State<TInstance> superStateInstance = GetState(superState.Name);

            var property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var stateProperty = statePropertyExpression.GetPropertyInfo();

            var name = $"{property.Name}.{stateProperty.Name}";

            StateMachineState<TInstance> existingState = GetStateProperty(stateProperty, propertyValue);
            if (name.Equals(existingState?.Name) && superState.Name.Equals(existingState?.SuperState?.Name))
                return;

            var state = new StateMachineState<TInstance>((c, s) => UnhandledEvent(c, s), name, _eventObservers, superStateInstance);

            ConfigurationHelpers.InitializeStateProperty(stateProperty, propertyValue, state);

            SetState(name, state);
        }

        /// <summary>
        /// Adds the state, and state transition events, to the cache
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        void SetState(string name, StateMachineState<TInstance> state)
        {
            _stateCache[name] = state;

            _eventCache[state.BeforeEnter.Name] = new StateMachineEvent<TInstance>(state.BeforeEnter, true);
            _eventCache[state.Enter.Name] = new StateMachineEvent<TInstance>(state.Enter, true);
            _eventCache[state.Leave.Name] = new StateMachineEvent<TInstance>(state.Leave, true);
            _eventCache[state.AfterLeave.Name] = new StateMachineEvent<TInstance>(state.AfterLeave, true);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified state
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state, params EventActivities<TInstance>[] activities)
        {
            IActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="state1">The state</param>
        /// <param name="state2">The other state</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state1, State state2, params EventActivities<TInstance>[] activities)
        {
            IActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, activitiesBinder);
            BindActivitiesToState(state2, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="state1">The state</param>
        /// <param name="state2">The other state</param>
        /// <param name="state3">The other other state</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state1, State state2, State state3, params EventActivities<TInstance>[] activities)
        {
            IActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, activitiesBinder);
            BindActivitiesToState(state2, activitiesBinder);
            BindActivitiesToState(state3, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="state1">The state</param>
        /// <param name="state2">The other state</param>
        /// <param name="state3">The other other state</param>
        /// <param name="state4">Okay, this is getting a bit ridiculous at this point</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state1, State state2, State state3, State state4,
            params EventActivities<TInstance>[] activities)
        {
            IActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, activitiesBinder);
            BindActivitiesToState(state2, activitiesBinder);
            BindActivitiesToState(state3, activitiesBinder);
            BindActivitiesToState(state4, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="states">The states</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(IEnumerable<State> states, params EventActivities<TInstance>[] activities)
        {
            IActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (var state in states)
                BindActivitiesToState(state, activitiesBinder);
        }

        void BindActivitiesToState(State state, IActivityBinder<TInstance>[] eventActivities)
        {
            State<TInstance> activityState = GetState(state.Name);

            foreach (IActivityBinder<TInstance> activity in eventActivities)
            {
                activity.Bind(activityState);
                if (!_compositeEvents.Contains(activity.Event.Name) || !_compositeBindings.ContainsKey(activity.Event.Name))
                    continue;

                foreach (KeyValuePair<string, List<EventActivityBinder<TInstance>>> compositeBinding in _compositeBindings[activity.Event.Name])
                {
                    IActivityBinder<TInstance>[] activitiesBinder = compositeBinding.Value.SelectMany(x => x.GetStateActivityBinders()).ToArray();

                    if (!activitiesBinder.Any())
                        continue;

                    BindActivitiesToState(GetState(compositeBinding.Key), activitiesBinder);
                }

                _compositeBindings.Remove(activity.Event.Name);
            }
        }

        /// <summary>
        /// Declares the events and activities that are handled during the initial state
        /// </summary>
        /// <param name="activities">The event and activities</param>
        protected internal void Initially(params EventActivities<TInstance>[] activities)
        {
            During(Initial, activities);
        }

        /// <summary>
        /// Declares events and activities that are handled during any state except the Initial and Final
        /// </summary>
        /// <param name="activities">The event and activities</param>
        protected internal void DuringAny(params EventActivities<TInstance>[] activities)
        {
            IActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            IEnumerable<State<TInstance>> states = _stateCache.Values.Where(x => !Equals(x, Initial) && !Equals(x, Final));

            // We only add DuringAny event handlers to non-initial and non-final states to avoid
            // reviving finalized state machine instances or creating new ones accidentally.
            foreach (State<TInstance> state in states)
                BindActivitiesToState(state, activitiesBinder);

            // Specifically bind CompositeEvents to Initial and Final states to avoid them not being able to be fired.
            IActivityBinder<TInstance>[] compositeEvents = activitiesBinder.Where(binder => IsCompositeEvent(binder.Event)).ToArray();
            BindActivitiesToState(_initial, compositeEvents);
            BindActivitiesToState(_final, compositeEvents);

            BindTransitionEvents(_initial, activities);
            BindTransitionEvents(_final, activities);
        }

        /// <summary>
        /// When the Final state is entered, execute the chained activities. This occurs in any state that is not the initial or final state
        /// </summary>
        /// <param name="activityCallback">Specify the activities that are executes when the Final state is entered.</param>
        protected internal void Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            EventActivityBinder<TInstance> binder = When(Final.Enter);

            binder = activityCallback(binder);

            DuringAny(binder);
        }

        void BindTransitionEvents(State<TInstance> state, IEnumerable<EventActivities<TInstance>> activities)
        {
            IEnumerable<IActivityBinder<TInstance>> eventActivities = activities
                .SelectMany(activity => activity.GetStateActivityBinders().Where(x => x.IsStateTransitionEvent(state)));

            foreach (IActivityBinder<TInstance> eventActivity in eventActivities)
                eventActivity.Bind(state);
        }

        /// <summary>
        /// When the event is fired in this state, execute the chained activities
        /// </summary>
        /// <param name="event">The fired event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance> When(Event @event)
        {
            return When(@event, null);
        }

        /// <summary>
        /// When the event is fired in this state, and the event data matches the filter expression, execute the chained activities
        /// </summary>
        /// <param name="event">The fired event</param>
        /// <param name="filter">The filter applied to the event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance> When(Event @event, StateMachineCondition<TInstance> filter)
        {
            return new TriggerEventActivityBinder<TInstance>(this, @event, filter);
        }

        /// <summary>
        /// When entering the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenEnter(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(this, activityState.Enter);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// When entering any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenEnterAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.Enter);
        }

        /// <summary>
        /// When leaving any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenLeaveAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.Leave);
        }

        void BindEveryTransitionEvent(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback,
            Func<State<TInstance>, Event> eventProvider)
        {
            State<TInstance>[] states = _stateCache.Values.ToArray();

            IActivityBinder<TInstance>[] binders = states.Select(state =>
            {
                EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(this, eventProvider(state));

                return activityCallback(binder);
            }).SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (State<TInstance> state in states)
            {
                foreach (IActivityBinder<TInstance> binder in binders)
                    binder.Bind(state);
            }
        }

        /// <summary>
        /// Before entering any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void BeforeEnterAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.BeforeEnter);
        }

        /// <summary>
        /// After leaving any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void AfterLeaveAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.AfterLeave);
        }

        void BindEveryTransitionEvent(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback,
            Func<State<TInstance>, Event<State>> eventProvider)
        {
            State<TInstance>[] states = _stateCache.Values.ToArray();

            IActivityBinder<TInstance>[] binders = states.Select(state =>
            {
                EventActivityBinder<TInstance, State> binder = new DataEventActivityBinder<TInstance, State>(this, eventProvider(state));

                return activityCallback(binder);
            }).SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (State<TInstance> state in states)
            {
                foreach (IActivityBinder<TInstance> binder in binders)
                    binder.Bind(state);
            }
        }

        /// <summary>
        /// When leaving the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenLeave(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(this, activityState.Leave);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// Before entering the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void BeforeEnter(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

            EventActivityBinder<TInstance, State> binder = new DataEventActivityBinder<TInstance, State>(this, activityState.BeforeEnter);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// After leaving the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void AfterLeave(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

            EventActivityBinder<TInstance, State> binder = new DataEventActivityBinder<TInstance, State>(this, activityState.AfterLeave);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// When the event is fired in this state, execute the chained activities
        /// </summary>
        /// <typeparam name="TMessage">The event data type</typeparam>
        /// <param name="event">The fired event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance, TMessage> When<TMessage>(Event<TMessage> @event)
            where TMessage : class
        {
            return When(@event, null);
        }

        /// <summary>
        /// When the event is fired in this state, and the event data matches the filter expression, execute the chained activities
        /// </summary>
        /// <typeparam name="TMessage">The event data type</typeparam>
        /// <param name="event">The fired event</param>
        /// <param name="filter">The filter applied to the event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance, TMessage> When<TMessage>(Event<TMessage> @event, StateMachineCondition<TInstance, TMessage> filter)
            where TMessage : class
        {
            return new DataEventActivityBinder<TInstance, TMessage>(this, @event, filter);
        }

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <param name="event">The ignored event</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore(Event @event)
        {
            IActivityBinder<TInstance> activityBinder = new IgnoreEventActivityBinder<TInstance>(@event);

            return new TriggerEventActivityBinder<TInstance>(this, @event, activityBinder);
        }

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The ignored event</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> @event)
            where TData : class
        {
            IActivityBinder<TInstance> activityBinder = new IgnoreEventActivityBinder<TInstance>(@event);

            return new DataEventActivityBinder<TInstance, TData>(this, @event, activityBinder);
        }

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The ignored event</param>
        /// <param name="filter">The filter to apply to the event data</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> @event, StateMachineCondition<TInstance, TData> filter)
            where TData : class
        {
            IActivityBinder<TInstance> activityBinder = new IgnoreEventActivityBinder<TInstance, TData>(@event, filter);

            return new DataEventActivityBinder<TInstance, TData>(this, @event, activityBinder);
        }

        /// <summary>
        /// Specifies a callback to invoke when an event is raised in a state where the event is not handled
        /// </summary>
        /// <param name="callback">The unhandled event callback</param>
        protected internal void OnUnhandledEvent(UnhandledEventCallback<TInstance> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _unhandledEventCallback = callback;
        }

        Task UnhandledEvent(BehaviorContext<TInstance> context, State state)
        {
            var unhandledEventContext = new UnhandledEventBehaviorContext<TInstance>(this, context, state);

            return _unhandledEventCallback(unhandledEventContext);
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
        protected internal void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression,
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
        protected internal void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression,
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
        protected internal void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression,
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
        protected internal void Request<TRequest, TResponse, TResponse2, TResponse3>(
            Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression,
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
        protected internal void Request<TRequest, TResponse, TResponse2, TResponse3>(
            Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression,
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
        protected internal void Schedule<TMessage>(Expression<Func<Schedule<TInstance, TMessage>>> propertyExpression,
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
                        Guid? tokenId = schedule.GetTokenId(context.Saga);

                        if (context.TryGetPayload(out ConsumeContext consumeContext))
                        {
                            Guid? messageTokenId = consumeContext.GetSchedulingTokenId();
                            if (messageTokenId.HasValue)
                            {
                                if (!tokenId.HasValue || messageTokenId.Value != tokenId.Value)
                                {
                                    LogContext.Debug?.Log("SAGA: {CorrelationId} Scheduled message not current: {TokenId}", context.Saga.CorrelationId,
                                        messageTokenId.Value);

                                    return;
                                }
                            }
                        }

                        BehaviorContext<TInstance, TMessage> eventContext = context.CreateProxy(schedule.Received, context.Message);

                        await ((StateMachine<TInstance>)this).RaiseEvent(eventContext).ConfigureAwait(false);

                        if (schedule.GetTokenId(context.Saga) == tokenId)
                            schedule.SetTokenId(context.Saga, default);
                    }));
        }

        static Task<bool> NotCompletedByDefault(BehaviorContext<TInstance> instance)
        {
            return TaskUtil.False;
        }

        static void InitializeSchedule<T>(MassTransitStateMachine<TInstance> stateMachine, PropertyInfo property, Schedule<TInstance, T> schedule)
            where T : class
        {
            if (property.CanWrite)
                property.SetValue(stateMachine, schedule);
            else if (ConfigurationHelpers.TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                backingField.SetValue(stateMachine, schedule);
            else
                throw new ArgumentException($"The schedule property is not writable: {property.Name}");
        }

        static void InitializeRequest<TRequest, TResponse>(MassTransitStateMachine<TInstance> stateMachine, PropertyInfo property,
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
            foreach (var declaration in _registrations.Value)
                declaration.Declare(this);

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
            var registrationType = messageType.HasInterface<CorrelatedBy<Guid>>()
                ? typeof(CorrelatedEventRegistration<>).MakeGenericType(typeof(TInstance), messageType)
                : typeof(UncorrelatedEventRegistration<>).MakeGenericType(typeof(TInstance), messageType);

            return (EventRegistration)Activator.CreateInstance(registrationType, @event);
        }

        StateMachine<TInstance> Modify(Action<IStateMachineModifier<TInstance>> modifier)
        {
            IStateMachineModifier<TInstance> builder = new StateMachineModifier<TInstance>(this);
            modifier(builder);
            builder.Apply();

            return this;
        }

        /// <summary>
        /// Create a new state machine using the builder pattern
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static MassTransitStateMachine<TInstance> New(Action<IStateMachineModifier<TInstance>> modifier)
        {
            var machine = new BuilderStateMachine();
            machine.Modify(modifier);
            return machine;
        }


        protected static class ConfigurationHelpers
        {
            public static StateMachineRegistration[] GetRegistrations(MassTransitStateMachine<TInstance> stateMachine)
            {
                var events = new List<StateMachineRegistration>();

                var machineType = stateMachine.GetType().GetTypeInfo();

                IEnumerable<PropertyInfo> properties = GetStateMachineProperties(machineType);

                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        if (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                        {
                            var declarationType = typeof(DataEventRegistration<,>).MakeGenericType(typeof(TInstance), machineType,
                                propertyInfo.PropertyType.GetGenericArguments().First());
                            var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                            events.Add((StateMachineRegistration)declaration);
                        }
                    }
                    else
                    {
                        if (propertyInfo.PropertyType == typeof(Event))
                        {
                            var declarationType = typeof(TriggerEventRegistration<>).MakeGenericType(typeof(TInstance), machineType);
                            var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                            events.Add((StateMachineRegistration)declaration);
                        }
                        else if (propertyInfo.PropertyType == typeof(State))
                        {
                            var declarationType = typeof(StateRegistration<>).MakeGenericType(typeof(TInstance), machineType);
                            var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                            events.Add((StateMachineRegistration)declaration);
                        }
                    }
                }

                return events.ToArray();
            }

            public static IEnumerable<PropertyInfo> GetStateMachineProperties(TypeInfo typeInfo)
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
                    .Where(x => x.CanRead && (x.CanWrite || TryGetBackingField(typeInfo, x, out _)));

                foreach (var propertyInfo in properties)
                    yield return propertyInfo;
            }

            public static bool TryGetBackingField(TypeInfo typeInfo, PropertyInfo property, out FieldInfo backingField)
            {
                backingField = typeInfo
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .FirstOrDefault(field =>
                        field.Attributes.HasFlag(FieldAttributes.Private) &&
                        field.Attributes.HasFlag(FieldAttributes.InitOnly) &&
                        field.CustomAttributes.Any(attr => attr.AttributeType == typeof(CompilerGeneratedAttribute)) &&
                        field.DeclaringType == property.DeclaringType &&
                        field.FieldType.IsAssignableFrom(property.PropertyType) &&
                        field.Name.StartsWith("<" + property.Name + ">")
                    );

                return backingField != null;
            }

            public static void InitializeState(MassTransitStateMachine<TInstance> stateMachine, PropertyInfo property,
                StateMachineState<TInstance> state)
            {
                if (property.CanWrite)
                    property.SetValue(stateMachine, state);
                else if (TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                    backingField.SetValue(stateMachine, state);
                else
                    throw new ArgumentException($"The state property is not writable: {property.Name}");
            }

            public static void InitializeStateProperty<TProperty>(PropertyInfo stateProperty, TProperty propertyValue,
                StateMachineState<TInstance> state)
                where TProperty : class
            {
                if (stateProperty.CanWrite)
                    stateProperty.SetValue(propertyValue, state);
                else
                {
                    var objectProperty = propertyValue.GetType().GetProperty(stateProperty.Name, typeof(State));
                    if (objectProperty == null || !objectProperty.CanWrite)
                        throw new ArgumentException($"The state property is not writable: {stateProperty.Name}");

                    objectProperty.SetValue(propertyValue, state);
                }
            }

            public static void InitializeEvent(MassTransitStateMachine<TInstance> stateMachine, PropertyInfo property, Event @event)
            {
                if (property.CanWrite)
                    property.SetValue(stateMachine, @event);
                else if (TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                    backingField.SetValue(stateMachine, @event);
                else
                    throw new ArgumentException($"The event property is not writable: {property.Name}");
            }

            public static void InitializeEventProperty<TProperty, T>(PropertyInfo eventProperty, TProperty propertyValue, Event @event)
                where TProperty : class
                where T : class
            {
                if (eventProperty.CanWrite)
                    eventProperty.SetValue(propertyValue, @event);
                else
                {
                    var objectProperty = propertyValue.GetType().GetProperty(eventProperty.Name, typeof(Event<T>));
                    if (objectProperty == null || !objectProperty.CanWrite)
                        throw new ArgumentException($"The event property is not writable: {eventProperty.Name}");

                    objectProperty.SetValue(propertyValue, @event);
                }
            }


            public interface StateMachineRegistration
            {
                void Declare(object stateMachine);
            }


            class StateRegistration<TStateMachine> :
                StateMachineRegistration
                where TStateMachine : MassTransitStateMachine<TInstance>
            {
                readonly PropertyInfo _propertyInfo;

                public StateRegistration(PropertyInfo propertyInfo)
                {
                    _propertyInfo = propertyInfo;
                }

                public void Declare(object stateMachine)
                {
                    var machine = (TStateMachine)stateMachine;
                    var existing = _propertyInfo.GetValue(machine);
                    if (existing != null)
                        return;

                    machine.DeclareState(_propertyInfo);
                }
            }


            class TriggerEventRegistration<TStateMachine> :
                StateMachineRegistration
                where TStateMachine : MassTransitStateMachine<TInstance>
            {
                readonly PropertyInfo _propertyInfo;

                public TriggerEventRegistration(PropertyInfo propertyInfo)
                {
                    _propertyInfo = propertyInfo;
                }

                public void Declare(object stateMachine)
                {
                    var machine = (TStateMachine)stateMachine;
                    var existing = _propertyInfo.GetValue(machine);
                    if (existing != null)
                        return;

                    machine.DeclarePropertyBasedEvent(prop => machine.DeclareTriggerEvent(prop.Name), _propertyInfo);
                }
            }


            class DataEventRegistration<TStateMachine, TData> :
                StateMachineRegistration
                where TStateMachine : MassTransitStateMachine<TInstance>
                where TData : class
            {
                readonly PropertyInfo _propertyInfo;

                public DataEventRegistration(PropertyInfo propertyInfo)
                {
                    _propertyInfo = propertyInfo;
                }

                public void Declare(object stateMachine)
                {
                    var machine = (TStateMachine)stateMachine;
                    var existing = _propertyInfo.GetValue(machine);
                    if (existing != null)
                        return;

                    machine.DeclarePropertyBasedEvent(prop => machine.DeclareDataEvent<TData>(prop.Name), _propertyInfo);
                }
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


        class BuilderStateMachine :
            MassTransitStateMachine<TInstance>
        {
        }


        interface EventRegistration
        {
            void RegisterCorrelation(MassTransitStateMachine<TInstance> machine);
        }
    }
}
