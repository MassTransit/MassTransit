namespace MassTransit.PrometheusIntegration.Observers
{
    using Configuration;
    using ConsumeConfigurators;


    public class PrometheusConsumerConfigurationObserver :
        IConsumerConfigurationObserver
    {
        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new PrometheusConsumerSpecification<T, TMessage>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
