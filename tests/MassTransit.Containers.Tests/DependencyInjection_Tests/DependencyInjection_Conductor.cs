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

        protected override void ConfigureServiceEndpoints(IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator> configurator)
        {
            configurator.ConfigureServiceEndpoints(GetRegistrationContext(), Options);
        }

        IRegistrationContext GetRegistrationContext()
        {
            return new RegistrationContext(_provider.GetRequiredService<IRegistration>(), _provider.GetRequiredService<BusHealth>());
        }

        protected override IClientFactory GetClientFactory()
        {
            return _provider.GetRequiredService<IClientFactory>();
        }
    }
}
