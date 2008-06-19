namespace MassTransit.ServiceBus.Subscriptions.ServerHandlers
{
    using System;
    using log4net;
    using Messages;

    public class RemoveSubscriptionHandler :
        Consumes<RemoveSubscription>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RemoveSubscriptionHandler));
        private readonly ISubscriptionCache _cache;
        private readonly ISubscriptionRepository _repository;
        private readonly FollowerRepository _followers;


        public RemoveSubscriptionHandler(ISubscriptionCache cache, ISubscriptionRepository repository, FollowerRepository followers)
        {
            _cache = cache;
            _repository = repository;
            _followers = followers;
        }

        public void Consume(RemoveSubscription message)
        {
            try
            {
                _cache.Remove(message.Subscription);

                _repository.Remove(message.Subscription);

                _followers.NotifyFollowers(message);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling subscription change", ex);
            }
        }   
    }
}