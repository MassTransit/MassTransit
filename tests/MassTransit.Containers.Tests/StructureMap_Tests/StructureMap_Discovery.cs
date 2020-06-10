namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using System;
    using Common_Tests;
    using Common_Tests.Discovery;
    using NUnit.Framework;
    using StructureMap;
    using TestFramework.Messages;


    public class StructureMap_Discovery :
        Common_Discovery
    {
        readonly IContainer _container;

        public StructureMap_Discovery()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(x =>
                {
                    x.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
                    x.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
                    x.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
                    x.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

                    x.AddBus(provider => BusControl);
                    x.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
                });

                registry.RegisterInMemorySagaRepository<DiscoveryPingSaga>();
                registry.RegisterInMemorySagaRepository<DiscoveryPingState>();
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRequestClient<PingMessage> GetRequestClient()
        {
            return _container.GetInstance<IRequestClient<PingMessage>>();
        }
    }
}
