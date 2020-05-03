namespace MassTransit.SignalR.Utils
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.AspNetCore.SignalR;


    public class MassTransitSubscriptionManager
    {
        readonly ConcurrentDictionary<string, HubConnectionStore> _subscriptions = new ConcurrentDictionary<string, HubConnectionStore>(StringComparer.Ordinal);

        public HubConnectionStore this[string identifier]
        {
            get
            {
                _subscriptions.TryGetValue(identifier, out var connectionStore);
                return connectionStore;
            }
        }

        public int Count => _subscriptions.Count;

        public void AddSubscription(string id, HubConnectionContext connection)
        {
            var subscription = _subscriptions.GetOrAdd(id, _ => new HubConnectionStore());

            subscription.Add(connection);
        }

        public void RemoveSubscription(string id, HubConnectionContext connection)
        {
            if (!_subscriptions.TryGetValue(id, out var subscription))
            {
                return;
            }

            subscription.Remove(connection);
        }
    }
}
