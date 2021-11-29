namespace MassTransit.SagaStateMachine
{
    public interface IBehaviorBuilder<TInstance>
        where TInstance : class, ISaga
    {
        void Add(IStateMachineActivity<TInstance> activity);
    }
}
