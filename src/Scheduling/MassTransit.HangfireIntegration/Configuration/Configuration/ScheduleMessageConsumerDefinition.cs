namespace MassTransit.Configuration
{
    using HangfireIntegration;
    using Scheduling;


    public class ScheduleMessageConsumerDefinition :
        ConsumerDefinition<ScheduleMessageConsumer>
    {
        readonly HangfireEndpointDefinition _endpointDefinition;

        public ScheduleMessageConsumerDefinition(HangfireEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ScheduleMessageConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 250));

            consumerConfigurator.Message<ScheduleMessage>(m => m.UsePartitioner(_endpointDefinition.Partition, p => p.Message.CorrelationId));
            consumerConfigurator.Message<CancelScheduledMessage>(m => m.UsePartitioner(_endpointDefinition.Partition, p => p.Message.CorrelationId));
        }
    }
}
