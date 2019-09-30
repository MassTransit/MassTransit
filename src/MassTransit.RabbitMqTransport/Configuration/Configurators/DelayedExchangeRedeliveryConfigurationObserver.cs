namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;
    using GreenPipes.Configurators;
    using PipeConfigurators;
    using Specifications;


    public class DelayedExchangeRedeliveryConfigurationObserver :
        ScheduledRedeliveryConfigurationObserver
    {
        public DelayedExchangeRedeliveryConfigurationObserver(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
            : base(configurator, configure)
        {
        }

        protected override void AddRedeliveryPipeSpecification<TMessage>(IConsumePipeConfigurator configurator)
        {
            var redeliverySpecification = new DelayedExchangeRedeliveryPipeSpecification<TMessage>();

            configurator.AddPipeSpecification(redeliverySpecification);
        }
    }
}
