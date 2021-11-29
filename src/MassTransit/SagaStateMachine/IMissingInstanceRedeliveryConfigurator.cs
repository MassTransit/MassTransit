namespace MassTransit
{
    using System;


    public interface IMissingInstanceRedeliveryConfigurator<TInstance, TData> :
        IRedeliveryConfigurator
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        void OnRedeliveryLimitReached(Func<IMissingInstanceConfigurator<TInstance, TData>, IPipe<ConsumeContext<TData>>> configure);
    }
}
