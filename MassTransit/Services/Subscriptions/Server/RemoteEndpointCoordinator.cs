namespace MassTransit.Services.Subscriptions.Server
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using MassTransit.Subscriptions;
    using Messages;

    public class RemoteEndpointCoordinator :
        Consumes<AddSubscription>.All,
        Consumes<RemoveSubscription>.All,
        Consumes<CancelSubscriptionUpdates>.All,
        Consumes<CacheUpdateRequest>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (RemoteEndpointCoordinator));
        private readonly ISubscriptionCache _cache;
        private readonly FollowerRepository _followers;
        private readonly ISubscriptionRepository _repository;
        private readonly IEndpointFactory _endpointFactory;

        public RemoteEndpointCoordinator(ISubscriptionCache cache, FollowerRepository followers, ISubscriptionRepository repository, IEndpointFactory endpointFactory)
        {
            _cache = cache;
            _endpointFactory = endpointFactory;
            _followers = followers;
            _repository = repository;
        }

        public void Consume(AddSubscription message)
        {
         
            try
            {
                _cache.Add(Convert(message.Subscription));

                _repository.Save(Convert(message.Subscription));

                _followers.NotifyFollowers(message);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error processing subscription change (add) for the location of '{0}' for the message '{1}' ", message.Subscription.EndpointUri, message.Subscription.MessageName);
                _log.Error(msg, ex);
            }
        
        }

        public void Consume(RemoveSubscription message)
        {
        
            try
            {
                _cache.Remove(Convert(message.Subscription));

                _repository.Remove(Convert(message.Subscription));

                _followers.NotifyFollowers(message);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error processing subscription change (remove) for the location of '{0}' for the message '{1}' ", message.Subscription.EndpointUri, message.Subscription.MessageName);
                _log.Error(msg, ex);
            }
        
        }

        public void Consume(CancelSubscriptionUpdates message)
        {
            try
            {
                _followers.RemoveFollower(message.RequestingUri);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error processing cancel subscription change for the location of '{0}'", message.RequestingUri);
                _log.Error(msg, ex);
            }
        }

        public void Consume(CacheUpdateRequest message)
        {
            try
            {
                _followers.AddFollower(message.RequestingUri);

                IList<Subscription> subscriptions = RemoveNHibernateness(_cache.List());

                CacheUpdateResponse response = new CacheUpdateResponse(subscriptions);

                IEndpoint ep = _endpointFactory.GetEndpoint(message.RequestingUri);
                ep.Send(response);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception handling cache update request from '{0}'", message.RequestingUri), ex);
            }
        }

        private static Subscription Convert(SubscriptionInformation info)
        {
            return new Subscription(info.MessageName, info.CorrelationId, info.EndpointUri);
        }

        /// <summary>
        /// The NHibernate objects don't serialize, so we rip that off here.
        /// </summary>
        private static IList<Subscription> RemoveNHibernateness(IEnumerable<Subscription> subs)
        {
            IList<Subscription> result = new List<Subscription>();

            foreach (Subscription sub in subs)
            {
                result.Add(new Subscription(sub));
            }

            return result;
        }
    }
}