namespace MassTransit.SignalR.Tests
{
    using MassTransit.SignalR.Consumers;
    using MassTransit.SignalR.Contracts;
    using MassTransit.Testing;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Internal;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using System;
    using Utils;


    public abstract class MassTransitHubLifetimeTestFixture<THub>
        where THub : Hub
    {
        protected abstract BusTestHarness Harness { get; set; }

        protected SignalRBackplaneConsumersTestHarness<THub> RegisterBusEndpoint(string queueName = "receiveEndpoint")
        {
            var consumersTestHarness = new SignalRBackplaneConsumersTestHarness<THub>(Harness, queueName);

            consumersTestHarness.AddAllConsumer(new HubLifetimeManagerConsumerFactory<AllConsumer<THub>, THub>(hubMgr => new AllConsumer<THub>(hubMgr)));
            consumersTestHarness.AddConnectionConsumer(new HubLifetimeManagerConsumerFactory<ConnectionConsumer<THub>, THub>(hubMgr => new ConnectionConsumer<THub>(hubMgr)));
            consumersTestHarness.AddGroupConsumer(new HubLifetimeManagerConsumerFactory<GroupConsumer<THub>, THub>(hubMgr => new GroupConsumer<THub>(hubMgr)));
            consumersTestHarness.AddGroupManagementConsumer(new HubLifetimeManagerConsumerFactory<GroupManagementConsumer<THub>, THub>(hubMgr => new GroupManagementConsumer<THub>(hubMgr)));
            consumersTestHarness.AddUserConsumer(new HubLifetimeManagerConsumerFactory<UserConsumer<THub>, THub>(hubMgr => new UserConsumer<THub>(hubMgr)));

            return consumersTestHarness;
        }

        public HubLifetimeManager<THub> CreateLifetimeManager(MessagePackHubProtocolOptions messagePackOptions = null, JsonHubProtocolOptions jsonOptions = null)
        {
            messagePackOptions = messagePackOptions ?? new MessagePackHubProtocolOptions();
            jsonOptions = jsonOptions ?? new JsonHubProtocolOptions();

            var manager = new MassTransitHubLifetimeManager<THub>(
            Harness.Bus,
            Harness.Bus.CreateClientFactory(TimeSpan.FromSeconds(5)),
            new DefaultHubProtocolResolver(new IHubProtocol[]
            {
                new JsonHubProtocol(Options.Create(jsonOptions)),
                new MessagePackHubProtocol(Options.Create(messagePackOptions)),
            }, NullLogger<DefaultHubProtocolResolver>.Instance));

            return manager;
        }
    }
}
