namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using Common_Tests.Discovery;
    using NUnit.Framework;
    using TestFramework.Messages;


    public class Autofac_Discovery :
        Common_Discovery
    {
        readonly IContainer _container;

        public Autofac_Discovery()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

                x.AddBus(provider => BusControl);
                x.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
            });

            builder.RegisterInMemorySagaRepository<DiscoveryPingSaga>();
            builder.RegisterInMemorySagaRepository<DiscoveryPingState>();

            _container = builder.Build();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IRequestClient<PingMessage> GetRequestClient()
        {
            return _container.Resolve<IRequestClient<PingMessage>>();
        }
    }
}
