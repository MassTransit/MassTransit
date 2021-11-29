namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class TransitionActivity<TSaga> :
        IStateMachineActivity<TSaga>
        where TSaga : class, ISaga
    {
        readonly IStateAccessor<TSaga> _currentStateAccessor;
        readonly State<TSaga> _toState;

        public TransitionActivity(State<TSaga> toState, IStateAccessor<TSaga> currentStateAccessor)
        {
            _toState = toState;
            _currentStateAccessor = currentStateAccessor;
        }

        public State ToState => _toState;

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transition");
            scope.Add("toState", _toState.Name);
        }

        public async Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
        {
            await Transition(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<TData>(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
            where TData : class
        {
            await Transition(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where T : class
            where TException : Exception
        {
            return next.Faulted(context);
        }

        async Task Transition(BehaviorContext<TSaga> context)
        {
            State<TSaga> currentState = await _currentStateAccessor.Get(context).ConfigureAwait(false);
            if (_toState.Equals(currentState))
                return; // Homey don't play re-entry, at least not yet.

            if (currentState != null && !currentState.HasState(_toState))
                await RaiseCurrentStateLeaveEvents(context, currentState).ConfigureAwait(false);

            await RaiseBeforeEnterEvents(context, currentState, _toState).ConfigureAwait(false);

            await _currentStateAccessor.Set(context, _toState).ConfigureAwait(false);

            if (currentState != null)
                await RaiseAfterLeaveEvents(context, currentState, _toState).ConfigureAwait(false);

            if (currentState == null || !_toState.HasState(currentState))
            {
                State<TSaga> superState = _toState.SuperState;
                while (superState != null && (currentState == null || !superState.HasState(currentState)))
                {
                    BehaviorContext<TSaga> superStateEnterContext = context.CreateProxy(superState.Enter);
                    await superState.Raise(superStateEnterContext).ConfigureAwait(false);

                    superState = superState.SuperState;
                }

                BehaviorContext<TSaga> enterContext = context.CreateProxy(_toState.Enter);
                await _toState.Raise(enterContext).ConfigureAwait(false);
            }
        }

        async Task RaiseBeforeEnterEvents(BehaviorContext<TSaga> context, State<TSaga> currentState, State<TSaga> toState)
        {
            State<TSaga> superState = toState.SuperState;
            if (superState != null && (currentState == null || !superState.HasState(currentState)))
                await RaiseBeforeEnterEvents(context, currentState, superState).ConfigureAwait(false);

            if (currentState != null && toState.HasState(currentState))
                return;

            BehaviorContext<TSaga, State> beforeContext = context.CreateProxy(toState.BeforeEnter, toState);
            await toState.Raise(beforeContext).ConfigureAwait(false);
        }

        async Task RaiseAfterLeaveEvents(BehaviorContext<TSaga> context, State<TSaga> fromState, State<TSaga> toState)
        {
            if (fromState.HasState(toState))
                return;

            BehaviorContext<TSaga, State> afterContext = context.CreateProxy(fromState.AfterLeave, fromState);
            await fromState.Raise(afterContext).ConfigureAwait(false);

            State<TSaga> superState = fromState.SuperState;
            if (superState != null)
                await RaiseAfterLeaveEvents(context, superState, toState).ConfigureAwait(false);
        }

        async Task RaiseCurrentStateLeaveEvents(BehaviorContext<TSaga> context, State<TSaga> fromState)
        {
            BehaviorContext<TSaga> leaveContext = context.CreateProxy(fromState.Leave);
            await fromState.Raise(leaveContext).ConfigureAwait(false);

            State<TSaga> superState = fromState.SuperState;
            while (superState != null && !superState.HasState(_toState))
            {
                BehaviorContext<TSaga> superStateLeaveContext = context.CreateProxy(superState.Leave);
                await superState.Raise(superStateLeaveContext).ConfigureAwait(false);

                superState = superState.SuperState;
            }
        }
    }
}
