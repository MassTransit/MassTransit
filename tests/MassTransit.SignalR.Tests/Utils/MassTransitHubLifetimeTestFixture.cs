namespace MassTransit.SignalR.Tests
{
    using System;
    using Consumers;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Internal;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Testing;
    using Utils;


    public abstract class MassTransitHubLifetimeTestFixture<THub>
        where THub : Hub
    {
        readonly string _prefix;

        protected MassTransitHubLifetimeTestFixture()
        {
            _prefix = Environment.MachineName;
        }

        protected abstract BusTestHarness Harness { get; set; }

        protected SignalRBackplaneConsumersTestHarness<THub> RegisterBusEndpoint(string queueName = "receiveEndpoint")
        {
            var consumersTestHarness = new SignalRBackplaneConsumersTestHarness<THub>(Harness, queueName);

            consumersTestHarness.AddAllConsumer(new HubLifetimeManagerConsumerFactory<AllConsumer<THub>, THub>(hubMgr => new AllConsumer<THub>(hubMgr)));
            consumersTestHarness.AddConnectionConsumer(
                new HubLifetimeManagerConsumerFactory<ConnectionConsumer<THub>, THub>(hubMgr => new ConnectionConsumer<THub>(hubMgr)));
            consumersTestHarness.AddGroupConsumer(new HubLifetimeManagerConsumerFactory<GroupConsumer<THub>, THub>(hubMgr => new GroupConsumer<THub>(hubMgr)));
            consumersTestHarness.AddGroupManagementConsumer(
                new HubLifetimeManagerConsumerFactory<GroupManagementConsumer<THub>, THub>(hubMgr => new GroupManagementConsumer<THub>(hubMgr)));
            consumersTestHarness.AddUserConsumer(new HubLifetimeManagerConsumerFactory<UserConsumer<THub>, THub>(hubMgr => new UserConsumer<THub>(hubMgr)));

            return consumersTestHarness;
        }

        protected MassTransitHubLifetimeManager<THub> CreateLifetimeManager(MessagePackHubProtocolOptions messagePackOptions = null,
            JsonHubProtocolOptions jsonOptions = null)
        {
            messagePackOptions ??= new MessagePackHubProtocolOptions();
            jsonOptions ??= new JsonHubProtocolOptions();

            var manager = new MassTransitHubLifetimeManager<THub>(
                new HubLifetimeManagerOptions<THub> {ServerName = $"{_prefix}_{Guid.NewGuid():N}"},
                new BusHubLifetimeScopeProvider(Harness.Bus),
                new DefaultHubProtocolResolver(
                    new IHubProtocol[] {new JsonHubProtocol(Options.Create(jsonOptions)), new MessagePackHubProtocol(Options.Create(messagePackOptions)),},
                    NullLogger<DefaultHubProtocolResolver>.Instance)
            );

            return manager;
        }
    }
}
