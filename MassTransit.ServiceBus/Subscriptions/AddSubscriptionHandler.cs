namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using log4net;
    using Messages;

    public class AddSubscriptionHandler :
        Consumes<AddSubscription>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AddSubscriptionHandler));
        private readonly ISubscriptionCache _cache;
        private readonly ISubscriptionRepository _repository;
        private readonly FollowerRepository _followers;


        public AddSubscriptionHandler(ISubscriptionCache cache, ISubscriptionRepository repository, FollowerRepository followers)
        {
            _cache = cache;
            _repository = repository;
            _followers = followers;
        }

        public void Consume(AddSubscription message)
        {
            try
            {
                _cache.Add(message.Subscription);

                _repository.Save(message.Subscription);

                _followers.NotifyFollowers(message);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling subscription change", ex);
            }
        }
    }
}