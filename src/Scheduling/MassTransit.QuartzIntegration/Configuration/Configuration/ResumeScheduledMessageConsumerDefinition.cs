namespace MassTransit.Configuration
{
    using QuartzIntegration;
    using Scheduling;


    public class ResumeScheduledMessageConsumerDefinition :
        ConsumerDefinition<ResumeScheduledMessageConsumer>
    {
        readonly QuartzEndpointDefinition _endpointDefinition;

        public ResumeScheduledMessageConsumerDefinition(QuartzEndpointDefinition endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;

            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ResumeScheduledMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.Message<ResumeScheduledRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.ScheduleGroup},{p.Message.ScheduleId}");
            });
        }
    }
}
