namespace MassTransit
{
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class StateMachineExtensions
    {
        /// <summary>
        /// Transition a state machine instance to a specific state, producing any events related
        /// to the transaction such as leaving the previous state and entering the target state
        /// </summary>
        /// <typeparam name="TSaga">The state instance type</typeparam>
        /// <param name="context"></param>
        /// <param name="state">The target state</param>
        public static Task TransitionToState<TSaga>(this BehaviorContext<TSaga> context, State state)
            where TSaga : class, ISaga
        {
            IStateAccessor<TSaga> accessor = context.StateMachine.Accessor;
            State<TSaga> toState = context.StateMachine.GetState(state.Name);

            IStateMachineActivity<TSaga> activity = new TransitionActivity<TSaga>(toState, accessor);
            IBehavior<TSaga> behavior = new LastBehavior<TSaga>(activity);

            return behavior.Execute(context.CreateProxy(toState.Enter));
        }
    }
}
