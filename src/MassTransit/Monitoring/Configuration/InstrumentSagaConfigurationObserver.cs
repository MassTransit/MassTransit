namespace MassTransit.Monitoring.Configuration
{
    public class InstrumentSagaConfigurationObserver :
        ISagaConfigurationObserver
    {
        public void SagaConfigured<T>(ISagaConfigurator<T> configurator)
            where T : class, ISaga
        {
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
        }

        public void SagaMessageConfigured<T, TMessage>(ISagaMessageConfigurator<T, TMessage> configurator)
            where T : class, ISaga
            where TMessage : class
        {
            var specification = new InstrumentSagaSpecification<T, TMessage>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
