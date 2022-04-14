namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using MassTransit.Tests.ReliableMessaging;


    public class ReliableStateDefinition :
        SagaDefinition<ReliableState>
    {
        readonly IServiceProvider _provider;

        public ReliableStateDefinition(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
            ISagaConfigurator<ReliableState> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

            endpointConfigurator.UseMessageScope(_provider);
            endpointConfigurator.UseEntityFrameworkOutbox<ReliableDbContext>(_provider);
        }
    }
}
