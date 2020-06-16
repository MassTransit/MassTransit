namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Common_Tests.JobConsumerContracts;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjection_JobConsumer :
        Common_JobConsumer
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_JobConsumer()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override void ConfigureServiceEndpoints(IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator> configurator)
        {
            configurator.ConfigureServiceEndpoints(_provider.GetRequiredService<IBusRegistrationContext>(), Options);
        }

        protected override IRequestClient<CrunchTheNumbers> RequestClient => _provider.CreateRequestClient<CrunchTheNumbers>();
    }
}
