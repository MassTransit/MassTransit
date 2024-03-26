namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class GraphStateMachineVisitor<TSaga> :
        StateMachineVisitor
        where TSaga : class, SagaStateMachineInstance
    {
        readonly HashSet<Edge> _edges;
        readonly Dictionary<Event, Vertex> _events;
        readonly StateMachine<TSaga> _machine;
        readonly Dictionary<State, Vertex> _states;
        Edge _currentEdge;
        Vertex _currentEvent;
        Vertex _currentState;

        public GraphStateMachineVisitor(StateMachine<TSaga> machine)
        {
            _machine = machine;

            _edges = new HashSet<Edge>();
            _states = new Dictionary<State, Vertex>();
            _events = new Dictionary<Event, Vertex>();
        }

        public StateMachineGraph Graph
        {
            get
            {
                IEnumerable<Vertex> events = _events.Values
                    .Where(e => _edges.Any(edge => edge.From.Equals(e)));

                IEnumerable<Vertex> states = _states.Values
                    .Where(s => _edges.Any(edge => edge.From.Equals(s) || edge.To.Equals(s)));

                var vertices = new HashSet<Vertex>(states.Union(events));

                IEnumerable<Edge> edges = _edges
                    .Where(e => vertices.Contains(e.From) && vertices.Contains(e.To));

                return new StateMachineGraph(vertices, edges);
            }
        }

        public void Visit(State state, Action<State> next)
        {
            _currentState = GetStateVertex(state);

            next(state);
        }

        public void Visit(Event @event, Action<Event> next)
        {
            _currentEvent = GetEventVertex(@event);
            _currentEdge = null;

            next(@event);
        }

        public void Visit<TData>(Event<TData> @event, Action<Event<TData>> next)
            where TData : class
        {
            _currentEvent = GetEventVertex(@event);
            _currentEdge = null;

            next(@event);
        }

        public void Visit(IStateMachineActivity activity)
        {
            Visit(activity, x =>
            {
            });
        }

        public void Visit<T>(IBehavior<T> behavior)
            where T : class, SagaStateMachineInstance
        {
            Visit(behavior, x =>
            {
            });
        }

        public void Visit<T>(IBehavior<T> behavior, Action<IBehavior<T>> next)
            where T : class, SagaStateMachineInstance
        {
            next(behavior);
        }

        public void Visit<T, TData>(IBehavior<T, TData> behavior)
            where T : class, SagaStateMachineInstance
            where TData : class
        {
            Visit(behavior, x =>
            {
            });
        }

        public void Visit<T, TData>(IBehavior<T, TData> behavior, Action<IBehavior<T, TData>> next)
            where T : class, SagaStateMachineInstance
            where TData : class
        {
            next(behavior);
        }

        public void Visit(IStateMachineActivity activity, Action<IStateMachineActivity> next)
        {
            if (activity is TransitionActivity<TSaga> transitionActivity)
            {
                InspectTransitionActivity(transitionActivity);
                next(activity);
                return;
            }

            if (activity is CompositeEventActivity<TSaga> compositeActivity)
            {
                InspectCompositeEventActivity(compositeActivity);
                next(activity);
                return;
            }

            var activityType = activity.GetType();
            var compensateType = activityType.IsGenericType
                && activityType.GetGenericTypeDefinition() == typeof(CatchFaultActivity<,>)
                    ? activityType.GetGenericArguments().Skip(1).First()
                    : null;

            if (compensateType != null)
            {
                AddCurrentEdge();

                var previousEvent = _currentEvent;

                var eventType = typeof(MessageEvent<>).MakeGenericType(compensateType);
                var evt = (Event)Activator.CreateInstance(eventType, compensateType.Name);
                _currentEvent = GetEventVertex(evt);

                _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));

                next(activity);

                _currentEvent = previousEvent;
                return;
            }

            next(activity);
        }

        void AddCurrentEdge()
        {
            if (_currentEvent.IsComposite || _currentEdge != null)
                return;

            _currentEdge = new Edge(_currentState, _currentEvent, _currentEvent.Title);
            _edges.Add(_currentEdge);
        }

        void InspectTransitionActivity(TransitionActivity<TSaga> transitionActivity)
        {
            AddCurrentEdge();

            var targetState = GetStateVertex(transitionActivity.ToState);

            _edges.Add(new Edge(_currentEvent, targetState, _currentEvent.Title));
        }

        void InspectCompositeEventActivity(CompositeEventActivity<TSaga> compositeActivity)
        {
            AddCurrentEdge();

            var compositeEvent = GetEventVertex(compositeActivity.Event);

            _edges.Add(new Edge(_currentEvent, compositeEvent, compositeEvent.Title));
        }

        Vertex GetStateVertex(State state)
        {
            if (_states.TryGetValue(state, out var vertex))
                return vertex;

            vertex = CreateStateVertex(state);
            _states.Add(state, vertex);

            return vertex;
        }

        Vertex GetEventVertex(Event state)
        {
            if (_events.TryGetValue(state, out var vertex))
                return vertex;

            vertex = CreateEventVertex(state);
            _events.Add(state, vertex);

            return vertex;
        }

        static Vertex CreateStateVertex(State state)
        {
            return new Vertex(typeof(State), typeof(State), state.Name, false);
        }

        Vertex CreateEventVertex(Event @event)
        {
            var targetType = @event
                .GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
                .Select(x => x.GetGenericArguments()[0])
                .DefaultIfEmpty(typeof(Event))
                .Single();

            return new Vertex(typeof(Event), targetType, @event.Name, _machine.IsCompositeEvent(@event));
        }

        static Vertex CreateEventVertex(Type exceptionType)
        {
            return new Vertex(typeof(Event), exceptionType, exceptionType.Name, false);
        }
    }
}
