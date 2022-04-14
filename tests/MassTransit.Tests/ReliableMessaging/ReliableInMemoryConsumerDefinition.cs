namespace MassTransit.Tests.ReliableMessaging
{
    using System;


    public class ReliableInMemoryConsumerDefinition :
        ConsumerDefinition<ReliableConsumer>
    {
        readonly IServiceProvider _provider;

        public ReliableInMemoryConsumerDefinition(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReliableConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

            endpointConfigurator.UseInMemoryInboxOutbox(_provider);
        }
    }
}
