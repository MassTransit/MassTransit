namespace MassTransit.PrometheusIntegration.Configuration
{
    using System;
    using Internals;


    public class PrometheusConsumerConfigurationObserver :
        IConsumerConfigurationObserver
    {
        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            if (typeof(TMessage).ClosesType(typeof(Batch<>), out Type[] types))
            {
                typeof(PrometheusConsumerConfigurationObserver)
                    .GetMethod(nameof(BatchConsumerConfigured))
                    .MakeGenericMethod(typeof(TConsumer), types[0])
                    .Invoke(this, new object[] {configurator});
            }
            else
            {
                var specification = new PrometheusConsumerSpecification<TConsumer, TMessage>();

                configurator.AddPipeSpecification(specification);
            }
        }

        public void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            var specification = new PrometheusConsumerSpecification<TConsumer, Batch<TMessage>>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
