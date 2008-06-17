/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus.Subscriptions
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using Internal;
	using log4net;
	using Messages;

	public class SubscriptionService :
		IHostedService,
        Consumes<CacheUpdateRequest>.All,
        Consumes<AddSubscription>.All,
        Consumes<RemoveSubscription>.All,
        Consumes<CancelSubscriptionUpdates>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly ISubscriptionRepository _repository;
	    private readonly IEndpointResolver _endpointResolver;
	    private readonly IList<Uri> _followers;


		public SubscriptionService(IServiceBus bus, ISubscriptionCache subscriptionCache, ISubscriptionRepository subscriptionRepository, IEndpointResolver endpointResolver)
		{
		    _followers = new List<Uri>();
			_bus = bus;
		    _endpointResolver = endpointResolver;
		    _cache = subscriptionCache;
			_repository = subscriptionRepository;
		}

		#region IDisposable Members

        public void Dispose()
        {
            try
            {
                _bus.Dispose();
                _cache.Dispose();
                _repository.Dispose();
            }
            catch (Exception ex)
            {
                string message = "Error in shutting down the SubscriptionService: " + ex.Message;
                _log.Error(message);
                throw new ShutDownException(message, ex);
            }
        }

	    #endregion

		public void Start()
		{
            if (_log.IsInfoEnabled)
                _log.Info("Subscription Service Starting");

			foreach (Subscription sub in _repository.List())
			{
				_cache.Add(sub);
			}

            _bus.Subscribe(this);

            if(_log.IsInfoEnabled)
                _log.Info("Subscription Service Started");
		}

		public void Stop()
		{
            if (_log.IsInfoEnabled)
                _log.Info("Subscription Service Stopping");

            _bus.Unsubscribe(this);

            if (_log.IsInfoEnabled)
                _log.Info("Subscription Service Stopped");
		}


	    public void Consume(AddSubscription message)
	    {
	        try
			{
				_cache.Add(message.Subscription);

				_repository.Save(message.Subscription);

			    NotifyFollowers(message);
			}
			catch (Exception ex)
			{
				_log.Error("Exception handling subscription change", ex);
			}
	    }


        public void Consume(RemoveSubscription message)
        {
            try
            {
                _cache.Remove(message.Subscription);

                _repository.Remove(message.Subscription);

                NotifyFollowers(message);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling subscription change", ex);
            }
        }

		public void Consume(CacheUpdateRequest message)
	    {
	        try
			{
				AddFollower(message.RequestingUri);
				
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


		public void Consume(CancelSubscriptionUpdates message)
	    {
			RemoveFollower(message.RequestingUri);
		}

		private void AddFollower(Uri uri)
		{
			lock(_followers)
			{
				if (_followers.Contains(uri))
					return;

				_followers.Add(uri);
			}
		}

		private void RemoveFollower(Uri uri)
		{
			lock(_followers)
			{
				if (_followers.Contains(uri))
					_followers.Remove(uri);
			}
		}

		private void NotifyFollowers<T>(T message) where T : SubscriptionChange
		{
			IList<Uri> copy;
			lock(_followers)
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