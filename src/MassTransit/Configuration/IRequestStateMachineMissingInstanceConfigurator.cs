namespace MassTransit
{
    public interface IRequestStateMachineMissingInstanceConfigurator
    {
        IPipe<ConsumeContext<TMessage>> Apply<TInstance, TMessage>(IMissingInstanceConfigurator<TInstance, TMessage> configurator)
            where TInstance : SagaStateMachineInstance
            where TMessage : class;
    }
}
