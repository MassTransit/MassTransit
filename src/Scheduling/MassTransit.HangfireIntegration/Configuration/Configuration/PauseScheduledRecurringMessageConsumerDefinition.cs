namespace MassTransit.Configuration
{
    using MassTransit.Scheduling;
    using MassTransit.HangfireIntegration;


    public class PauseScheduledRecurringMessageConsumerDefinition :
        ConsumerDefinition<PauseScheduledRecurringMessageConsumer>
    {
        readonly HangfireEndpointDefinition _endpointDefinition;

        public PauseScheduledRecurringMessageConsumerDefinition(HangfireEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<PauseScheduledRecurringMessageConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 250));

            consumerConfigurator.Message<PauseScheduledRecurringMessage>(m => m.UsePartitioner(_endpointDefinition.Partition, p => p.Message.CorrelationId));
        }
    }
}
