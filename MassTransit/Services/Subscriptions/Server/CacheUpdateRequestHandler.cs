namespace MassTransit.Subscriptions.ServerHandlers
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using log4net;
    using Messages;

    public class CacheUpdateRequestHandler :
        Consumes<CacheUpdateRequest>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CacheUpdateRequestHandler));
        private readonly FollowerRepository _followers;
        private readonly IEndpointResolver _endpointResolver;
        private readonly ISubscriptionCache _cache;


        public CacheUpdateRequestHandler(FollowerRepository followers, IEndpointResolver endpointResolver, ISubscriptionCache cache)
        {
            _followers = followers;
            _endpointResolver = endpointResolver;
            _cache = cache;
        }

        public void Consume(CacheUpdateRequest message)
        {
            try
            {
                _followers.AddFollower(message.RequestingUri);

                IList<Subscription> subscriptions = RemoveNHibernateness(_cache.List());

                CacheUpdateResponse response = new CacheUpdateResponse(subscriptions);

                IEndpoint ep = _endpointResolver.Resolve(message.RequestingUri);
                ep.Send(response);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling cache update request", ex);
            }
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