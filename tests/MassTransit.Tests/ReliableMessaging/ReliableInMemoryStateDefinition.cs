namespace MassTransit.Tests.ReliableMessaging
{
    using System;


    public class ReliableInMemoryStateDefinition :
        SagaDefinition<ReliableState>
    {
        readonly IServiceProvider _provider;

        public ReliableInMemoryStateDefinition(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
            ISagaConfigurator<ReliableState> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

            endpointConfigurator.UseMessageScope(_provider);
            endpointConfigurator.UseInMemoryInboxOutbox(_provider);
        }
    }
}
