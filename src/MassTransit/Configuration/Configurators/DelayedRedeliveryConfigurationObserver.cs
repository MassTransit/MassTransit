namespace MassTransit.Configurators
{
    using System;
    using GreenPipes.Configurators;
    using PipeConfigurators;


    public class DelayedRedeliveryConfigurationObserver :
        ScheduledRedeliveryConfigurationObserver
    {
        public DelayedRedeliveryConfigurationObserver(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
            : base(configurator, configure)
        {
        }

        protected override void AddRedeliveryPipeSpecification<TMessage>(IConsumePipeConfigurator configurator)
        {
            var redeliverySpecification = new DelayedRedeliveryPipeSpecification<TMessage>();

            configurator.AddPipeSpecification(redeliverySpecification);
        }
    }
}
