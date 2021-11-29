namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public class InitialIfNullStateAccessor<TSaga> :
        IStateAccessor<TSaga>
        where TSaga : class, ISaga
    {
        readonly IBehavior<TSaga> _initialBehavior;
        readonly IStateAccessor<TSaga> _stateAccessor;

        public InitialIfNullStateAccessor(State<TSaga> initialState, IStateAccessor<TSaga> stateAccessor)
        {
            _stateAccessor = stateAccessor;

            IStateMachineActivity<TSaga> initialActivity = new TransitionActivity<TSaga>(initialState, _stateAccessor);
            _initialBehavior = new LastBehavior<TSaga>(initialActivity);
        }

        async Task<State<TSaga>> IStateAccessor<TSaga>.Get(BehaviorContext<TSaga> context)
        {
            State<TSaga> state = await _stateAccessor.Get(context).ConfigureAwait(false);
            if (state == null)
            {
                await _initialBehavior.Execute(context).ConfigureAwait(false);

                state = await _stateAccessor.Get(context).ConfigureAwait(false);
            }

            return state;
        }

        Task IStateAccessor<TSaga>.Set(BehaviorContext<TSaga> context, State<TSaga> state)
        {
            return _stateAccessor.Set(context, state);
        }

        public Expression<Func<TSaga, bool>> GetStateExpression(params State[] states)
        {
            return _stateAccessor.GetStateExpression(states);
        }

        public void Probe(ProbeContext context)
        {
            _stateAccessor.Probe(context);
        }
    }
}
