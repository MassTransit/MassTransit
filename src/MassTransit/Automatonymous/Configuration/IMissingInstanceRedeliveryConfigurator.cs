namespace Automatonymous
{
    using System;
    using GreenPipes;
    using GreenPipes.Configurators;
    using MassTransit;


    public interface IMissingInstanceRedeliveryConfigurator<TInstance, TData> :
        IRetryConfigurator
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        void OnRedeliveryLimitReached(Func<IMissingInstanceConfigurator<TInstance, TData>, IPipe<ConsumeContext<TData>>> configure);
    }
}