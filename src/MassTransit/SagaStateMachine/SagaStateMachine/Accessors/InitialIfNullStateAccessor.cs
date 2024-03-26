namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        class InitialIfNullStateAccessor :
            IStateAccessor<TInstance>
        {
            readonly IBehavior<TInstance> _initialBehavior;
            readonly IStateAccessor<TInstance> _stateAccessor;

            public InitialIfNullStateAccessor(State<TInstance> initialState, IStateAccessor<TInstance> stateAccessor)
            {
                _stateAccessor = stateAccessor;

                IStateMachineActivity<TInstance> initialActivity = new TransitionActivity<TInstance>(initialState, _stateAccessor);
                _initialBehavior = new LastBehavior<TInstance>(initialActivity);
            }

            async Task<State<TInstance>> IStateAccessor<TInstance>.Get(BehaviorContext<TInstance> context)
            {
                State<TInstance> state = await _stateAccessor.Get(context).ConfigureAwait(false);
                if (state == null)
                {
                    await _initialBehavior.Execute(context).ConfigureAwait(false);

                    state = await _stateAccessor.Get(context).ConfigureAwait(false);
                }

                return state;
            }

            Task IStateAccessor<TInstance>.Set(BehaviorContext<TInstance> context, State<TInstance> state)
            {
                return _stateAccessor.Set(context, state);
            }

            public Expression<Func<TInstance, bool>> GetStateExpression(params State[] states)
            {
                return _stateAccessor.GetStateExpression(states);
            }

            public void Probe(ProbeContext context)
            {
                _stateAccessor.Probe(context);
            }
        }
    }
}
