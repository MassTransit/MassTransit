namespace MassTransit.Configuration
{
    using HangfireIntegration;


    public class ScheduleRecurringMessageConsumerDefinition :
        ConsumerDefinition<ScheduleRecurringMessageConsumer>
    {
        public ScheduleRecurringMessageConsumerDefinition(HangfireEndpointDefinition endpointDefinition)
        {
            EndpointDefinition = endpointDefinition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ScheduleRecurringMessageConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 250));
        }
    }
}
