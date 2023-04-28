namespace MassTransit.Configuration
{
    using QuartzIntegration;
    using Scheduling;


    public class CancelScheduledMessageConsumerDefinition :
        ConsumerDefinition<CancelScheduledMessageConsumer>
    {
        readonly QuartzEndpointDefinition _endpointDefinition;

        public CancelScheduledMessageConsumerDefinition(QuartzEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<CancelScheduledMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.Message<CancelScheduledMessage>(m => m.UsePartitioner(_endpointDefinition.Partition, p => p.Message.TokenId));

            consumerConfigurator.Message<CancelScheduledRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.ScheduleGroup},{p.Message.ScheduleId}");
            });
        }
    }
}
