namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    /// <summary>
    /// Accesses the current state as a string property
    /// </summary>
    /// <typeparam name="TSaga">The instance type</typeparam>
    public class IntStateAccessor<TSaga> :
        IStateAccessor<TSaga>
        where TSaga : class, ISaga
    {
        readonly StateAccessorIndex<TSaga> _index;
        readonly IStateObserver<TSaga> _observer;
        readonly PropertyInfo _propertyInfo;
        readonly IReadProperty<TSaga, int> _read;
        readonly IWriteProperty<TSaga, int> _write;

        public IntStateAccessor(Expression<Func<TSaga, int>> currentStateExpression, StateAccessorIndex<TSaga> index, IStateObserver<TSaga> observer)
        {
            _index = index;
            _observer = observer;

            _propertyInfo = currentStateExpression.GetPropertyInfo();

            _read = ReadPropertyCache<TSaga>.GetProperty<int>(_propertyInfo);
            _write = WritePropertyCache<TSaga>.GetProperty<int>(_propertyInfo);
        }

        Task<State<TSaga>> IStateAccessor<TSaga>.Get(BehaviorContext<TSaga> context)
        {
            var stateIndex = _read.Get(context.Saga);

            return Task.FromResult(_index[stateIndex]);
        }

        Task IStateAccessor<TSaga>.Set(BehaviorContext<TSaga> context, State<TSaga> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            var stateIndex = _index[state.Name];

            var previousIndex = _read.Get(context.Saga);

            if (stateIndex == previousIndex)
                return Task.CompletedTask;

            _write.Set(context.Saga, stateIndex);

            State<TSaga> previousState = _index[previousIndex];

            return _observer.StateChanged(context, state, previousState);
        }

        public Expression<Func<TSaga, bool>> GetStateExpression(params State[] states)
        {
            if (states == null || states.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(states), "One or more states must be specified");

            var parameterExpression = Expression.Parameter(typeof(TSaga), "instance");

            var statePropertyExpression = Expression.Property(parameterExpression, _propertyInfo.GetMethod);

            var stateExpression = states.Select(state => Expression.Equal(statePropertyExpression, Expression.Constant(_index[(string)state.Name])))
                .Aggregate((left, right) => Expression.Or(left, right));

            return Expression.Lambda<Func<TSaga, bool>>(stateExpression, parameterExpression);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("currentStateProperty", _propertyInfo.Name);
        }
    }
}
