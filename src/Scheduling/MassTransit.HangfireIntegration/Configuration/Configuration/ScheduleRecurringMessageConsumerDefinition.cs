namespace MassTransit.Configuration
{
    using HangfireIntegration;
    using Scheduling;


    public class ScheduleRecurringMessageConsumerDefinition :
        ConsumerDefinition<ScheduleRecurringMessageConsumer>
    {
        readonly HangfireEndpointDefinition _endpointDefinition;

        public ScheduleRecurringMessageConsumerDefinition(HangfireEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ScheduleRecurringMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.Message<ScheduleRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.Schedule?.ScheduleGroup},{p.Message.Schedule?.ScheduleId}");
            });

            consumerConfigurator.Message<CancelScheduledRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.ScheduleGroup},{p.Message.ScheduleId}");
            });
        }
    }
}
