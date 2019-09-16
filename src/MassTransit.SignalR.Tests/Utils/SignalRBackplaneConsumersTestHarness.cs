namespace MassTransit.SignalR.Tests
{
    using MassTransit.SignalR.Consumers;
    using MassTransit.Testing;
    using Microsoft.AspNetCore.SignalR;
    using System.Collections.Generic;
    using Utils;


    public class SignalRBackplaneConsumersTestHarness<THub>
        where THub : Hub
    {
        public ConsumerTestHarness<AllConsumer<THub>> All { get; private set; }
        public ConsumerTestHarness<ConnectionConsumer<THub>> Connection { get; private set; }
        public ConsumerTestHarness<GroupConsumer<THub>> Group { get; private set; }
        public ConsumerTestHarness<GroupManagementConsumer<THub>> GroupManagement { get; private set; }
        public ConsumerTestHarness<UserConsumer<THub>> User { get; private set; }

        readonly IList<IHubManagerConsumerFactory<THub>> _hubManagerConsumerFactories;

        public HubLifetimeManager<THub> HubLifetimeManager { get; private set; }

        readonly string _queuePrefix;
        readonly BusTestHarness _testHarness;

        public SignalRBackplaneConsumersTestHarness(BusTestHarness testHarness, string queuePrefix)
        {
            _queuePrefix = queuePrefix;
            _testHarness = testHarness;
            _hubManagerConsumerFactories = new List<IHubManagerConsumerFactory<THub>>();
        }

        public void AddAllConsumer(HubLifetimeManagerConsumerFactory<AllConsumer<THub>, THub> hubConsumerFactory)
        {
            All = new ConsumerTestHarness<AllConsumer<THub>>(_testHarness, hubConsumerFactory, $"{_queuePrefix}-all-");

            _hubManagerConsumerFactories.Add(hubConsumerFactory);
        }

        public void AddConnectionConsumer(HubLifetimeManagerConsumerFactory<ConnectionConsumer<THub>, THub> hubConsumerFactory)
        {
            Connection = new ConsumerTestHarness<ConnectionConsumer<THub>>(_testHarness, hubConsumerFactory, $"{_queuePrefix}-connection-");

            _hubManagerConsumerFactories.Add(hubConsumerFactory);
        }

        public void AddGroupConsumer(HubLifetimeManagerConsumerFactory<GroupConsumer<THub>, THub> hubConsumerFactory)
        {
            Group = new ConsumerTestHarness<GroupConsumer<THub>>(_testHarness, hubConsumerFactory, $"{_queuePrefix}-group-");

            _hubManagerConsumerFactories.Add(hubConsumerFactory);
        }

        public void AddGroupManagementConsumer(HubLifetimeManagerConsumerFactory<GroupManagementConsumer<THub>, THub> hubConsumerFactory)
        {
            GroupManagement = new ConsumerTestHarness<GroupManagementConsumer<THub>>(_testHarness, hubConsumerFactory, $"{_queuePrefix}-groupmgmt-");

            _hubManagerConsumerFactories.Add(hubConsumerFactory);
        }

        public void AddUserConsumer(HubLifetimeManagerConsumerFactory<UserConsumer<THub>, THub> hubConsumerFactory)
        {
            User = new ConsumerTestHarness<UserConsumer<THub>>(_testHarness, hubConsumerFactory, $"{_queuePrefix}-user-");

            _hubManagerConsumerFactories.Add(hubConsumerFactory);
        }

        public void SetHubLifetimeManager(HubLifetimeManager<THub> hubLifetimeManager)
        {
            foreach (var factory in _hubManagerConsumerFactories)
            {
                factory.HubLifetimeManager = hubLifetimeManager;
            }

            HubLifetimeManager = hubLifetimeManager;
        }
    }
}
