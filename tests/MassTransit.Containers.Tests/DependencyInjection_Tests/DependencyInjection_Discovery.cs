namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Common_Tests.Discovery;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class DependencyInjection_Discovery :
        Common_Discovery
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Discovery()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

                x.AddBus(provider => BusControl);
                x.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
            });

            collection.RegisterInMemorySagaRepository<DiscoveryPingSaga>();
            collection.RegisterInMemorySagaRepository<DiscoveryPingState>();

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRequestClient<PingMessage> GetRequestClient()
        {
            return _provider.CreateRequestClient<PingMessage>();
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }
}
