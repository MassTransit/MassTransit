namespace MassTransit.Configuration
{
    using System;


    public class InMemoryOutboxSagaConfigurationObserver<TSaga> :
        ISagaConfigurationObserver
        where TSaga : class, ISaga
    {
        readonly ISagaConfigurator<TSaga> _configurator;
        readonly Action<IOutboxConfigurator> _configure;

        public InMemoryOutboxSagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, Action<IOutboxConfigurator> configure)
        {
            _configurator = configurator;
            _configure = configure;
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

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
