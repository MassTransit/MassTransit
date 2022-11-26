namespace MassTransit.Configuration
{
    using MassTransit.Scheduling;
    using MassTransit.HangfireIntegration;


    public class ResumeScheduledRecurringMessageConsumerDefinition :
        ConsumerDefinition<ResumeScheduledRecurringMessageConsumer>
    {
        readonly HangfireEndpointDefinition _endpointDefinition;

        public ResumeScheduledRecurringMessageConsumerDefinition(HangfireEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ResumeScheduledRecurringMessageConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 250));

            consumerConfigurator.Message<ResumeScheduledRecurringMessage>(m => m.UsePartitioner(_endpointDefinition.Partition, p => p.Message.CorrelationId));
        }
    }
}
