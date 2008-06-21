namespace MassTransit.ServiceBus.Subscriptions
{
    using System.Collections.Generic;

    public class InMemorySubscriptionRepository :
        ISubscriptionRepository
    {
        readonly IList<Subscription> _subscriptions = new List<Subscription>();

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        public void Save(Subscription subscription)
        {
            _subscriptions.Add(subscription);
        }

        public void Remove(Subscription subscription)
        {
            _subscriptions.Remove(subscription);
        }

        public IEnumerable<Subscription> List()
        {
            return _subscriptions;
        }
    }
}