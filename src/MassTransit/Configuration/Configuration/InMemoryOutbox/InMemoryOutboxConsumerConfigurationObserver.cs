namespace MassTransit.Configuration
{
    using System;


    public class InMemoryOutboxConsumerConfigurationObserver<TConsumer> :
        IConsumerConfigurationObserver
        where TConsumer : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;
        readonly Action<IOutboxConfigurator> _configure;

        public InMemoryOutboxConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, Action<IOutboxConfigurator> configure)
        {
            _configurator = configurator;
            _configure = configure;
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new InMemoryOutboxSpecification<TMessage>();

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
