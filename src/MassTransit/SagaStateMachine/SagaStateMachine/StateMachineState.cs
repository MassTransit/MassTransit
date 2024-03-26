namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public class StateMachineState :
            State<TInstance>,
            IEquatable<State>
        {
            readonly Dictionary<Event, ActivityBehaviorBuilder<TInstance>> _behaviors;
            readonly Dictionary<Event, IStateEventFilter<TInstance>> _ignoredEvents;
            readonly IEventObserver<TInstance> _observer;
            readonly HashSet<State<TInstance>> _subStates;
            readonly StateMachineUnhandledEventCallback<TInstance> _unhandledEventCallback;

            public StateMachineState(StateMachineUnhandledEventCallback<TInstance> unhandledEventCallback, string name, IEventObserver<TInstance> observer,
                State<TInstance> superState = null)
            {
                _unhandledEventCallback = unhandledEventCallback;
                Name = name;
                _observer = observer;

                _behaviors = new Dictionary<Event, ActivityBehaviorBuilder<TInstance>>();
                _ignoredEvents = new Dictionary<Event, IStateEventFilter<TInstance>>();

                Enter = new TriggerEvent(name + ".Enter");
                Ignore(Enter);
                Leave = new TriggerEvent(name + ".Leave");
                Ignore(Leave);

                BeforeEnter = new MessageEvent<State>(name + ".BeforeEnter");
                Ignore(BeforeEnter);
                AfterLeave = new MessageEvent<State>(name + ".AfterLeave");
                Ignore(AfterLeave);

                _subStates = new HashSet<State<TInstance>>();

                SuperState = superState;
                superState?.AddSubstate(this);
            }

            public bool Equals(State other)
            {
                return string.CompareOrdinal(Name, other?.Name ?? "") == 0;
            }

            public State<TInstance> SuperState { get; }
            public string Name { get; }

            public Event Enter { get; }
            public Event Leave { get; }
            public Event<State> BeforeEnter { get; }
            public Event<State> AfterLeave { get; }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this, _ =>
                {
                    foreach (KeyValuePair<Event, ActivityBehaviorBuilder<TInstance>> behavior in _behaviors)
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
                    foreach (State<TInstance> subState in _subStates)
                        subStateScope.Add("name", subState.Name);
                }

                if (_behaviors.Any())
                {
                    foreach (KeyValuePair<Event, ActivityBehaviorBuilder<TInstance>> behavior in _behaviors)
                    {
                        var eventScope = scope.CreateScope("event");
                        behavior.Key.Probe(eventScope);

                        behavior.Value.Behavior.Probe(eventScope.CreateScope("behavior"));
                    }
                }

                List<KeyValuePair<Event, IStateEventFilter<TInstance>>> ignored = _ignoredEvents.Where(x => IsRealEvent(x.Key)).ToList();
                if (ignored.Any())
                {
                    foreach (KeyValuePair<Event, IStateEventFilter<TInstance>> ignoredEvent in ignored)
                        ignoredEvent.Key.Probe(scope.CreateScope("event-ignored"));
                }
            }

            async Task State<TInstance>.Raise(BehaviorContext<TInstance> context)
            {
                if (!_behaviors.TryGetValue(context.Event, out ActivityBehaviorBuilder<TInstance> activities))
                {
                    if (_ignoredEvents.TryGetValue(context.Event, out IStateEventFilter<TInstance> filter) && filter.Filter(context))
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

            async Task State<TInstance>.Raise<T>(BehaviorContext<TInstance, T> context)
            {
                if (!_behaviors.TryGetValue(context.Event, out ActivityBehaviorBuilder<TInstance> activities))
                {
                    if (_ignoredEvents.TryGetValue(context.Event, out IStateEventFilter<TInstance> filter) && filter.Filter(context))
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

            public void Bind(Event @event, IStateMachineActivity<TInstance> activity)
            {
                if (!_behaviors.TryGetValue(@event, out ActivityBehaviorBuilder<TInstance> builder))
                {
                    builder = new ActivityBehaviorBuilder<TInstance>();
                    _behaviors.Add(@event, builder);
                }

                builder.Add(activity);
            }

            public void Ignore(Event @event)
            {
                _ignoredEvents[@event] = new AllStateEventFilter<TInstance>();
            }

            public void Ignore<T>(Event<T> @event, StateMachineCondition<TInstance, T> filter)
                where T : class
            {
                _ignoredEvents[@event] = new SelectedStateEventFilter<TInstance, T>(filter);
            }

            public void AddSubstate(State<TInstance> subState)
            {
                if (subState == null)
                    throw new ArgumentNullException(nameof(subState));

                if (Name.Equals(subState.Name))
                    throw new ArgumentException("A state cannot be a substate of itself", nameof(subState));

                _subStates.Add(subState);
            }

            public bool HasState(State<TInstance> state)
            {
                return Name.Equals(state.Name) || _subStates.Any(s => s.HasState(state));
            }

            public bool IsStateOf(State<TInstance> state)
            {
                return Name.Equals(state.Name) || (SuperState != null && SuperState.IsStateOf(state));
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

            public static bool operator ==(State<TInstance> left, StateMachineState right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(State<TInstance> left, StateMachineState right)
            {
                return !Equals(left, right);
            }

            public static bool operator ==(StateMachineState left, State<TInstance> right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(StateMachineState left, State<TInstance> right)
            {
                return !Equals(left, right);
            }

            public static bool operator ==(StateMachineState left, StateMachineState right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(StateMachineState left, StateMachineState right)
            {
                return !Equals(left, right);
            }

            public override string ToString()
            {
                return $"{Name} (State)";
            }
        }
    }
}
