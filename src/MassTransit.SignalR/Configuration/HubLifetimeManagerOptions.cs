namespace MassTransit.SignalR
{
    using System;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


    public class HubLifetimeManagerOptions<THub> :
        IHubLifetimeManagerOptions<THub>
        where THub : Hub
    {
        public HubLifetimeManagerOptions()
        {
            ServerName = $"{Environment.MachineName}_{NewId.NextGuid():N}";
            RequestTimeout = TimeSpan.FromSeconds(20);
            ConnectionStore = new HubConnectionStore();
            GroupsSubscriptionManager = new MassTransitSubscriptionManager();
            UsersSubscriptionManager = new MassTransitSubscriptionManager();
        }

        public string ServerName { get; set; }
        public RequestTimeout RequestTimeout { get; set; }
        public HubConnectionStore ConnectionStore { get; }
        public MassTransitSubscriptionManager GroupsSubscriptionManager { get; }
        public MassTransitSubscriptionManager UsersSubscriptionManager { get; }
    }
}
