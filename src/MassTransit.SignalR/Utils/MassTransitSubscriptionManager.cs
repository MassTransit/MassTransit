namespace MassTransit.SignalR.Utils
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Concurrent;

    public class MassTransitSubscriptionManager
    {
        public readonly ConcurrentDictionary<string, HubConnectionStore> Subscriptions = new ConcurrentDictionary<string, HubConnectionStore>(StringComparer.Ordinal);

        public HubConnectionStore this[string identifier]
        {
            get
            {
                Subscriptions.TryGetValue(identifier, out var connectionStore);
                return connectionStore;
            }
        }

        public int Count => Subscriptions.Count;

        public void AddSubscription(string id, HubConnectionContext connection)
        {
            var subscription = Subscriptions.GetOrAdd(id, _ => new HubConnectionStore());

            subscription.Add(connection);
        }

        public void RemoveSubscription(string id, HubConnectionContext connection)
        {
            if (!Subscriptions.TryGetValue(id, out var subscription))
            {
                return;
            }

            subscription.Remove(connection);
        }
    }
}
