namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using MassTransit.Tests.ReliableMessaging;


    public class ReliableConsumerDefinition :
        ConsumerDefinition<ReliableConsumer>
    {
        readonly IServiceProvider _provider;

        public ReliableConsumerDefinition(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReliableConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

            endpointConfigurator.UseEntityFrameworkOutbox<ReliableDbContext>(_provider);
        }
    }
}
