namespace MassTransit.Configuration
{
    using HangfireIntegration;
    using Scheduling;


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
            IConsumerConfigurator<PauseScheduledRecurringMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.Message<PauseScheduledRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.ScheduleGroup},{p.Message.ScheduleId}");
            });
        }
    }
}
