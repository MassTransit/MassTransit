namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;


    /// <summary>
    /// The default state accessor will attempt to find and use a single State property on the
    /// instance type. If no State property is found, or more than one is found, an exception
    /// will be thrown
    /// </summary>
    public class DefaultInstanceStateAccessor<TSaga> :
        IStateAccessor<TSaga>
        where TSaga : class, ISaga
    {
        readonly Lazy<IStateAccessor<TSaga>> _accessor;
        readonly State<TSaga> _initialState;
        readonly StateMachine<TSaga> _machine;
        readonly IStateObserver<TSaga> _observer;

        public DefaultInstanceStateAccessor(StateMachine<TSaga> machine, State<TSaga> initialState, IStateObserver<TSaga> observer)
        {
            _machine = machine;
            _initialState = initialState;
            _observer = observer;
            _accessor = new Lazy<IStateAccessor<TSaga>>(CreateDefaultAccessor);
        }

        Task<State<TSaga>> IStateAccessor<TSaga>.Get(BehaviorContext<TSaga> context)
        {
            return _accessor.Value.Get(context);
        }

        Task IStateAccessor<TSaga>.Set(BehaviorContext<TSaga> context, State<TSaga> state)
        {
            return _accessor.Value.Set(context, state);
        }

        public Expression<Func<TSaga, bool>> GetStateExpression(params State[] states)
        {
            return _accessor.Value.GetStateExpression(states);
        }

        public void Probe(ProbeContext context)
        {
            _accessor.Value.Probe(context);
        }

        IStateAccessor<TSaga> CreateDefaultAccessor()
        {
            List<PropertyInfo> states = typeof(TSaga)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType == typeof(State))
                .Where(x => x.GetGetMethod(true) != null)
                .Where(x => x.GetSetMethod(true) != null)
                .ToList();

            if (states.Count > 1)
            {
                throw new SagaStateMachineException(
                    "The InstanceState was not configured, and could not be automatically identified as multiple State properties exist.");
            }

            if (states.Count == 0)
            {
                throw new SagaStateMachineException(
                    "The InstanceState was not configured, and no public State property exists.");
            }

            var instance = Expression.Parameter(typeof(TSaga), "instance");
            var memberExpression = Expression.Property(instance, states[0]);

            Expression<Func<TSaga, State>> expression = Expression.Lambda<Func<TSaga, State>>(memberExpression,
                instance);

            return new InitialIfNullStateAccessor<TSaga>(_initialState, new RawStateAccessor<TSaga>(_machine, expression, _observer));
        }
    }
}
