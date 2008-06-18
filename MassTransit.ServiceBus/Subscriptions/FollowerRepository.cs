namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using Messages;

    public class FollowerRepository
    {
        private readonly IList<Uri> _followers;
        private readonly IEndpointResolver _endpointResolver;


        public FollowerRepository(IEndpointResolver endpointResolver)
        {
            _followers = new List<Uri>();
            _endpointResolver = endpointResolver;
        }

        public void AddFollower(Uri uri)
        {
            lock (_followers)
            {
                if (_followers.Contains(uri))
                    return;

                _followers.Add(uri);
            }
        }

        public void RemoveFollower(Uri uri)
        {
            lock (_followers)
            {
                if (_followers.Contains(uri))
                    _followers.Remove(uri);
            }
        }

        public void NotifyFollowers<T>(T message) where T : SubscriptionChange
        {
            IList<Uri> copy;
            lock (_followers)
                copy = new List<Uri>(_followers);

            foreach (Uri uri in copy)
            {
                // don't send updates to the originator, that's chatty kathy
                if (message.Subscription.EndpointUri == uri)
                    continue;

                IEndpoint ep = _endpointResolver.Resolve(uri);
                ep.Send<T>(message);
            }
        }
    }
}