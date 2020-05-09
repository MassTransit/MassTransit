namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Registration;


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
            configurator.ConfigureServiceEndpoints(GetRegistrationContext(), Options);
        }

        IRegistrationContext<IServiceProvider> GetRegistrationContext()
        {
            return new RegistrationContext<IServiceProvider>(_provider.GetRequiredService<IRegistration>(), _provider.GetRequiredService<BusHealth>(), _provider);
        }

        protected override IClientFactory GetClientFactory()
        {
            return _provider.GetRequiredService<IClientFactory>();
        }
    }
}
