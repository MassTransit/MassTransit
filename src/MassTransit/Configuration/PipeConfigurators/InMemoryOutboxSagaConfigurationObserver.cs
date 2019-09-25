namespace MassTransit.PipeConfigurators
{
    using Automatonymous;
    using Saga;
    using SagaConfigurators;


    public class InMemoryOutboxSagaConfigurationObserver<TSaga> :
        ISagaConfigurationObserver
        where TSaga : class, ISaga
    {
        readonly ISagaConfigurator<TSaga> _configurator;

        public InMemoryOutboxSagaConfigurationObserver(ISagaConfigurator<TSaga> configurator)
        {
            _configurator = configurator;
        }

        void ISagaConfigurationObserver.SagaConfigured<T>(ISagaConfigurator<T> configurator)
        {
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<T, TMessage>(ISagaMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new InMemoryOutboxSpecification<TMessage>();

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
