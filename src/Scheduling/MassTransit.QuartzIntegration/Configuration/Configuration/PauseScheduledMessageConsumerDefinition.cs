namespace MassTransit.Configuration
{
    using QuartzIntegration;
    using Scheduling;


    public class PauseScheduledMessageConsumerDefinition :
        ConsumerDefinition<PauseScheduledMessageConsumer>
    {
        readonly QuartzEndpointDefinition _endpointDefinition;

        public PauseScheduledMessageConsumerDefinition(QuartzEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<PauseScheduledMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.Message<PauseScheduledRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.ScheduleGroup},{p.Message.ScheduleId}");
            });
        }
    }
}
