namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        /// <summary>
        /// Accesses the current state as a string property
        /// </summary>
        class IntStateAccessor :
            IStateAccessor<TInstance>
        {
            readonly StateAccessorIndex _index;
            readonly IStateObserver<TInstance> _observer;
            readonly PropertyInfo _propertyInfo;
            readonly IReadProperty<TInstance, int> _read;
            readonly IWriteProperty<TInstance, int> _write;

            public IntStateAccessor(Expression<Func<TInstance, int>> currentStateExpression, StateAccessorIndex index, IStateObserver<TInstance> observer)
            {
                _index = index;
                _observer = observer;

                _propertyInfo = currentStateExpression.GetPropertyInfo();

                _read = ReadPropertyCache<TInstance>.GetProperty<int>(_propertyInfo);
                _write = WritePropertyCache<TInstance>.GetProperty<int>(_propertyInfo);
            }

            Task<State<TInstance>> IStateAccessor<TInstance>.Get(BehaviorContext<TInstance> context)
            {
                var stateIndex = _read.Get(context.Saga);

                return Task.FromResult(_index[stateIndex]);
            }

            Task IStateAccessor<TInstance>.Set(BehaviorContext<TInstance> context, State<TInstance> state)
            {
                if (state == null)
                    throw new ArgumentNullException(nameof(state));

                var stateIndex = _index[state.Name];

                var previousIndex = _read.Get(context.Saga);

                if (stateIndex == previousIndex)
                    return Task.CompletedTask;

                _write.Set(context.Saga, stateIndex);

                State<TInstance> previousState = _index[previousIndex];

                return _observer.StateChanged(context, state, previousState);
            }

            public Expression<Func<TInstance, bool>> GetStateExpression(params State[] states)
            {
                if (states == null || states.Length == 0)
                    throw new ArgumentOutOfRangeException(nameof(states), "One or more states must be specified");

                var parameterExpression = Expression.Parameter(typeof(TInstance), "instance");

                var statePropertyExpression = Expression.Property(parameterExpression, _propertyInfo.GetMethod);

                var stateExpression = states.Select(state => Expression.Equal(statePropertyExpression, Expression.Constant(_index[state.Name])))
                    .Aggregate((left, right) => Expression.Or(left, right));

                return Expression.Lambda<Func<TInstance, bool>>(stateExpression, parameterExpression);
            }

            public void Probe(ProbeContext context)
            {
                context.Add("currentStateProperty", _propertyInfo.Name);
            }
        }
    }
}
