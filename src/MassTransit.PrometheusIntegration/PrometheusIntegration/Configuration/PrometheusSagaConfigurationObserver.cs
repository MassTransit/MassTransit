namespace MassTransit.PrometheusIntegration.Configuration
{
    public class PrometheusSagaConfigurationObserver :
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
            var specification = new PrometheusSagaSpecification<T, TMessage>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
