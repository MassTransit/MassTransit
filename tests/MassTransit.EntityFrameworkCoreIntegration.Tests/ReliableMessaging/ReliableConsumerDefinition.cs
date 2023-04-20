namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using MassTransit.Tests.ReliableMessaging;


    public class ReliableConsumerDefinition :
        ConsumerDefinition<ReliableConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReliableConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

            endpointConfigurator.UseEntityFrameworkOutbox<ReliableDbContext>(context);
        }
    }
}
