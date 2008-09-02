namespace MassTransit.ServiceBus.Subscriptions
{
    using System.Collections.Generic;
    using log4net;

    public class InMemorySubscriptionRepository :
        ISubscriptionRepository
    {
        private ILog _log = LogManager.GetLogger(typeof (InMemorySubscriptionRepository));
        readonly IList<Subscription> _subscriptions = new List<Subscription>();

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        public void Save(Subscription subscription)
        {
            _log.Info("Subscription Saved");
            _subscriptions.Add(subscription);
        }

        public void Remove(Subscription subscription)
        {
            _log.Info("Subscription REmoved");
            _subscriptions.Remove(subscription);
        }

        public IEnumerable<Subscription> List()
        {
            return _subscriptions;
        }
    }
}