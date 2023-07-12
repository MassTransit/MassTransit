namespace MassTransit.Configuration
{
    using HangfireIntegration;
    using Scheduling;


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
            IConsumerConfigurator<ResumeScheduledRecurringMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.Message<ResumeScheduledRecurringMessage>(m =>
            {
                m.UsePartitioner(_endpointDefinition.Partition, p => $"{p.Message.ScheduleGroup},{p.Message.ScheduleId}");
            });
        }
    }
}
