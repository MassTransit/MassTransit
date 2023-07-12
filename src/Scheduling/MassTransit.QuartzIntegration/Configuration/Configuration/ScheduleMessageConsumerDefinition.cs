namespace MassTransit.Configuration
{
    using QuartzIntegration;
    using Scheduling;


    public class ScheduleMessageConsumerDefinition :
        ConsumerDefinition<ScheduleMessageConsumer>
    {
        readonly QuartzEndpointDefinition _endpointDefinition;

        public ScheduleMessageConsumerDefinition(QuartzEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ScheduleMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 250));

            consumerConfigurator.Message<ScheduleMessage>(m => m.UsePartitioner(_endpointDefinition.Partition, p => p.Message.CorrelationId));

            consumerConfigurator.Message<ScheduleRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.Schedule?.ScheduleGroup},{p.Message.Schedule?.ScheduleId}");
            });
        }
    }
}
