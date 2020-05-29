namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using GreenPipes.Configurators;
    using PipeConfigurators;
    using Specifications;


    public class ActiveMqRedeliveryConfigurationObserver :
        ScheduledRedeliveryConfigurationObserver
    {
        public ActiveMqRedeliveryConfigurationObserver(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
            : base(configurator, configure)
        {
        }

        protected override void AddRedeliveryPipeSpecification<TMessage>(IConsumePipeConfigurator configurator)
        {
            var redeliverySpecification = new ActiveMqRedeliveryPipeSpecification<TMessage>();

            configurator.AddPipeSpecification(redeliverySpecification);
        }
    }
}
