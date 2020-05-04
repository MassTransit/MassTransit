namespace MassTransit.SignalR.Configuration.Definitions
{
    using System;
    using System.Globalization;
    using Consumers;
    using Microsoft.AspNetCore.SignalR;


    public class HubConsumerDefinition<THub> :
        IEndpointDefinition<AllConsumer<THub>>,
        IEndpointDefinition<ConnectionConsumer<THub>>,
        IEndpointDefinition<GroupConsumer<THub>>,
        IEndpointDefinition<GroupManagementConsumer<THub>>,
        IEndpointDefinition<UserConsumer<THub>>
        where THub : Hub
    {
        readonly Lazy<string> _hubName = new Lazy<string>(() => typeof(THub).Name.ToLower(CultureInfo.InvariantCulture));

        public bool IsTemporary => true;

        public int? PrefetchCount { get; private set; }

        public int? ConcurrentMessageLimit { get; private set; }

        public bool ConfigureConsumeTopology { get; private set; }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.TemporaryEndpoint($"signalr_{_hubName.Value}");
        }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}
