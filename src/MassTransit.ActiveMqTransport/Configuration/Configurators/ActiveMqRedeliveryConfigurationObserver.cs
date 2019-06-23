namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes.Configurators;
    using PipeConfigurators;
    using Specifications;

    public class ActiveMqRedeliveryConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Action<IRetryConfigurator> _configure;

        public ActiveMqRedeliveryConfigurationObserver(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
            : base(configurator)
        {
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var redeliverySpecification = new ActiveMqRedeliveryPipeSpecification<TMessage>();

            configurator.AddPipeSpecification(redeliverySpecification);

            var retrySpecification = new RedeliveryRetryPipeSpecification<TMessage>();

            _configure(retrySpecification);

            configurator.AddPipeSpecification(retrySpecification);
        }
    }
}
