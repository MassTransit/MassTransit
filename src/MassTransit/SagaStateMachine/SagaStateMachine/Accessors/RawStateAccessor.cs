namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    public class RawStateAccessor<TSaga> :
        IStateAccessor<TSaga>
        where TSaga : class, ISaga
    {
        readonly StateMachine<TSaga> _machine;
        readonly IStateObserver<TSaga> _observer;
        readonly PropertyInfo _propertyInfo;
        readonly IReadProperty<TSaga, State> _read;
        readonly IWriteProperty<TSaga, State> _write;

        public RawStateAccessor(StateMachine<TSaga> machine, Expression<Func<TSaga, State>> currentStateExpression, IStateObserver<TSaga> observer)
        {
            _machine = machine;
            _observer = observer;

            _propertyInfo = currentStateExpression.GetPropertyInfo();

            _read = ReadPropertyCache<TSaga>.GetProperty<State>(_propertyInfo);
            _write = WritePropertyCache<TSaga>.GetProperty<State>(_propertyInfo);
        }

        Task<State<TSaga>> IStateAccessor<TSaga>.Get(BehaviorContext<TSaga> context)
        {
            var state = _read.Get(context.Saga);
            if (state == null)
                return Task.FromResult<State<TSaga>>(null);

            return Task.FromResult(_machine.GetState(state.Name));
        }

        Task IStateAccessor<TSaga>.Set(BehaviorContext<TSaga> context, State<TSaga> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            var previous = _read.Get(context.Saga);
            if (state.Equals(previous))
                return Task.CompletedTask;

            _write.Set(context.Saga, state);

            State<TSaga> previousState = null;
            if (previous != null)
                previousState = _machine.GetState(previous.Name);

            return _observer.StateChanged(context, state, previousState);
        }

        public Expression<Func<TSaga, bool>> GetStateExpression(params State[] states)
        {
            if (states == null || states.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(states), "One or more states must be specified");

            var parameterExpression = Expression.Parameter(typeof(TSaga), "instance");

            var statePropertyExpression = Expression.Property(parameterExpression, _propertyInfo.GetMethod);

            var stateExpression = states.Select(state => Expression.Equal(statePropertyExpression,
                Expression.Constant(state, typeof(State)))).Aggregate((left, right) => Expression.Or(left, right));

            return Expression.Lambda<Func<TSaga, bool>>(stateExpression, parameterExpression);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("currentStateProperty", _propertyInfo.Name);
        }
    }
}
