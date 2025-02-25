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

        public int? PrefetchCount => default;

        public int? ConcurrentMessageLimit => default;

        public bool ConfigureConsumeTopology => true;

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.TemporaryEndpoint($"signalr_{_hubName.Value}");
        }

        public void Configure<T>(T configurator, IRegistrationContext context)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}
