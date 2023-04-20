namespace MassTransit.Tests.ReliableMessaging
{
    public class ReliableInMemoryConsumerDefinition :
        ConsumerDefinition<ReliableConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReliableConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

            endpointConfigurator.UseInMemoryInboxOutbox(context);
        }
    }
}
