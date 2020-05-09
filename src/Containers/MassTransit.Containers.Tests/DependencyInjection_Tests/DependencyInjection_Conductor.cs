namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjection_Conductor :
        Common_Conductor
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Conductor(bool instanceEndpoint)
            : base(instanceEndpoint)
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override void ConfigureServiceEndpoints(IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator> configurator)
        {
            configurator.ConfigureServiceEndpoints(_provider.GetRequiredService<IRegistrationContext<IServiceProvider>>(), Options);
        }

        protected override IClientFactory GetClientFactory()
        {
            return _provider.GetRequiredService<IClientFactory>();
        }
    }
}
