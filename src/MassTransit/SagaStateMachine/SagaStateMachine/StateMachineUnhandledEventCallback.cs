namespace MassTransit.SagaStateMachine
{
    using System.Threading.Tasks;


    public delegate Task StateMachineUnhandledEventCallback<TSaga>(BehaviorContext<TSaga> context, State state)
        where TSaga : class, SagaStateMachineInstance;
}
