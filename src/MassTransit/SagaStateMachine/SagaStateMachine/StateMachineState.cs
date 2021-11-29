namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class StateMachineState<TSaga> :
        State<TSaga>,
        IEquatable<State>
        where TSaga : class, SagaStateMachineInstance
    {
        readonly Dictionary<Event, ActivityBehaviorBuilder<TSaga>> _behaviors;
        readonly Dictionary<Event, IStateEventFilter<TSaga>> _ignoredEvents;
        readonly IEventObserver<TSaga> _observer;
        readonly HashSet<State<TSaga>> _subStates;
        readonly StateMachineUnhandledEventCallback<TSaga> _unhandledEventCallback;

        public StateMachineState(StateMachineUnhandledEventCallback<TSaga> unhandledEventCallback, string name, IEventObserver<TSaga> observer,
            State<TSaga> superState = null)
        {
            _unhandledEventCallback = unhandledEventCallback;
            Name = name;
            _observer = observer;

            _behaviors = new Dictionary<Event, ActivityBehaviorBuilder<TSaga>>();
            _ignoredEvents = new Dictionary<Event, IStateEventFilter<TSaga>>();

            Enter = new TriggerEvent(name + ".Enter");
            Ignore(Enter);
            Leave = new TriggerEvent(name + ".Leave");
            Ignore(Leave);

            BeforeEnter = new MessageEvent<State>(name + ".BeforeEnter");
            Ignore(BeforeEnter);
            AfterLeave = new MessageEvent<State>(name + ".AfterLeave");
            Ignore(AfterLeave);

            _subStates = new HashSet<State<TSaga>>();

            SuperState = superState;
            superState?.AddSubstate(this);
        }

        public bool Equals(State other)
        {
            return string.CompareOrdinal(Name, other?.Name ?? "") == 0;
        }

        public State<TSaga> SuperState { get; }
        public string Name { get; }

        public Event Enter { get; }
        public Event Leave { get; }
        public Event<State> BeforeEnter { get; }
        public Event<State> AfterLeave { get; }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, _ =>
            {
                foreach (KeyValuePair<Event, ActivityBehaviorBuilder<TSaga>> behavior in _behaviors)
                {
                    behavior.Key.Accept(visitor);
                    behavior.Value.Behavior.Accept(visitor);
                }
            });
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("state");
            scope.Add("name", Name);

            if (_subStates.Any())
            {
                var subStateScope = scope.CreateScope("substates");
                foreach (State<TSaga> subState in _subStates)
                    subStateScope.Add("name", subState.Name);
            }

            if (_behaviors.Any())
            {
                foreach (KeyValuePair<Event, ActivityBehaviorBuilder<TSaga>> behavior in _behaviors)
                {
                    var eventScope = scope.CreateScope("event");
                    behavior.Key.Probe(eventScope);

                    behavior.Value.Behavior.Probe(eventScope.CreateScope("behavior"));
                }
            }

            List<KeyValuePair<Event, IStateEventFilter<TSaga>>> ignored = _ignoredEvents.Where(x => IsRealEvent(x.Key)).ToList();
            if (ignored.Any())
            {
                foreach (KeyValuePair<Event, IStateEventFilter<TSaga>> ignoredEvent in ignored)
                    ignoredEvent.Key.Probe(scope.CreateScope("event-ignored"));
            }
        }

        async Task State<TSaga>.Raise(BehaviorContext<TSaga> context)
        {
            if (!_behaviors.TryGetValue(context.Event, out ActivityBehaviorBuilder<TSaga> activities))
            {
                if (_ignoredEvents.TryGetValue(context.Event, out IStateEventFilter<TSaga> filter) && filter.Filter(context))
                    return;

                if (SuperState != null)
                {
                    try
                    {
                        await SuperState.Raise(context).ConfigureAwait(false);
                        return;
                    }
                    catch (UnhandledEventException)
                    {
                        // the exception is better if it's from the substate
                    }
                }

                await _unhandledEventCallback(context, this).ConfigureAwait(false);
                return;
            }

            try
            {
                await _observer.PreExecute(context).ConfigureAwait(false);

                await activities.Behavior.Execute(context).ConfigureAwait(false);

                await _observer.PostExecute(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _observer.ExecuteFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        async Task State<TSaga>.Raise<T>(BehaviorContext<TSaga, T> context)
        {
            if (!_behaviors.TryGetValue(context.Event, out ActivityBehaviorBuilder<TSaga> activities))
            {
                if (_ignoredEvents.TryGetValue(context.Event, out IStateEventFilter<TSaga> filter) && filter.Filter(context))
                    return;

                if (SuperState != null)
                {
                    try
                    {
                        await SuperState.Raise(context).ConfigureAwait(false);
                        return;
                    }
                    catch (UnhandledEventException)
                    {
                        // the exception is better if it's from the substate
                    }
                }

                await _unhandledEventCallback(context, this).ConfigureAwait(false);
                return;
            }

            try
            {
                await _observer.PreExecute(context).ConfigureAwait(false);

                await activities.Behavior.Execute(context).ConfigureAwait(false);

                await _observer.PostExecute(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _observer.ExecuteFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        public void Bind(Event @event, IStateMachineActivity<TSaga> activity)
        {
            if (!_behaviors.TryGetValue(@event, out ActivityBehaviorBuilder<TSaga> builder))
            {
                builder = new ActivityBehaviorBuilder<TSaga>();
                _behaviors.Add(@event, builder);
            }

            builder.Add(activity);
        }

        public void Ignore(Event @event)
        {
            _ignoredEvents[@event] = new AllStateEventFilter<TSaga>();
        }

        public void Ignore<T>(Event<T> @event, StateMachineCondition<TSaga, T> filter)
            where T : class
        {
            _ignoredEvents[@event] = new SelectedStateEventFilter<TSaga, T>(filter);
        }

        public void AddSubstate(State<TSaga> subState)
        {
            if (subState == null)
                throw new ArgumentNullException(nameof(subState));

            if (Name.Equals(subState.Name))
                throw new ArgumentException("A state cannot be a substate of itself", nameof(subState));

            _subStates.Add(subState);
        }

        public bool HasState(State<TSaga> state)
        {
            return Name.Equals(state.Name) || _subStates.Any(s => s.HasState(state));
        }

        public bool IsStateOf(State<TSaga> state)
        {
            return Name.Equals(state.Name) || SuperState != null && SuperState.IsStateOf(state);
        }

        public IEnumerable<Event> Events => SuperState != null ? SuperState.Events.Union(GetStateEvents()).Distinct() : GetStateEvents();

        public int CompareTo(State other)
        {
            return string.CompareOrdinal(Name, other.Name);
        }

        bool IsRealEvent(Event @event)
        {
            if (Equals(@event, Enter) || Equals(@event, Leave) || Equals(@event, BeforeEnter) || Equals(@event, AfterLeave))
                return false;

            return true;
        }

        IEnumerable<Event> GetStateEvents()
        {
            return _behaviors.Keys
                .Union(_ignoredEvents.Keys)
                .Where(IsRealEvent)
                .Distinct();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var other = obj as State;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }

        public static bool operator ==(State<TSaga> left, StateMachineState<TSaga> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(State<TSaga> left, StateMachineState<TSaga> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(StateMachineState<TSaga> left, State<TSaga> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateMachineState<TSaga> left, State<TSaga> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(StateMachineState<TSaga> left, StateMachineState<TSaga> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateMachineState<TSaga> left, StateMachineState<TSaga> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Name} (State)";
        }
    }
}
