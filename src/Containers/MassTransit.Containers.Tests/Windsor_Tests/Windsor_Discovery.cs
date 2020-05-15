namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using System;
    using Castle.Windsor;
    using Common_Tests;
    using Common_Tests.Discovery;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class DependencyInjection_Discovery :
        Common_Discovery
    {
        readonly IWindsorContainer _container;

        public DependencyInjection_Discovery()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

                x.AddBus(provider => BusControl);
                x.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
            });

            _container.RegisterInMemorySagaRepository<DiscoveryPingSaga>();
            _container.RegisterInMemorySagaRepository<DiscoveryPingState>();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRequestClient<PingMessage> GetRequestClient()
        {
            return _container.Resolve<IRequestClient<PingMessage>>();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
    }
}
