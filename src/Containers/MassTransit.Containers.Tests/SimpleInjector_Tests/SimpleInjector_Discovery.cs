namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using Common_Tests;
    using Common_Tests.Discovery;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestFramework.Messages;


    [TestFixture]
    public class SimpleInjector_Discovery :
        Common_Discovery
    {
        readonly Container _container;

        public SimpleInjector_Discovery()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

                x.AddBus(() => BusControl);
                x.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
            });

            _container.RegisterInMemorySagaRepository<DiscoveryPingSaga>();
            _container.RegisterInMemorySagaRepository<DiscoveryPingState>();
        }

        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        protected override IRequestClient<PingMessage> GetRequestClient()
        {
            return _container.GetInstance<IRequestClient<PingMessage>>();
        }

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }
}
