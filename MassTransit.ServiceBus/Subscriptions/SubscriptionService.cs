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
	using Exceptions;
	using log4net;
	using ServerHandlers;

    public class SubscriptionService :
		IHostedService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly ISubscriptionRepository _repository;


		public SubscriptionService(IServiceBus bus, ISubscriptionCache subscriptionCache, ISubscriptionRepository subscriptionRepository)
		{
			_bus = bus;
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
                ShutDownException exp = new ShutDownException(message, ex);
                _log.Error(message, exp);
                throw exp;
            }
        }

	    #endregion

		public void Start()
		{
            _bus.AddComponent<AddSubscriptionHandler>();
            _bus.AddComponent<RemoveSubscriptionHandler>();
            _bus.AddComponent<CancelUpdatesHandler>();
            _bus.AddComponent<CacheUpdateRequestHandler>();

            if (_log.IsInfoEnabled)
                _log.Info("Subscription Service Starting");

			foreach (Subscription sub in _repository.List())
			{
				_cache.Add(sub);
			}

            if(_log.IsInfoEnabled)
                _log.Info("Subscription Service Started");
		}

		public void Stop()
		{
            if (_log.IsInfoEnabled)
                _log.Info("Subscription Service Stopping");


            _bus.RemoveComponent<AddSubscriptionHandler>();
            _bus.RemoveComponent<RemoveSubscriptionHandler>();
            _bus.RemoveComponent<CancelUpdatesHandler>();
            _bus.RemoveComponent<CacheUpdateRequestHandler>();

            if (_log.IsInfoEnabled)
                _log.Info("Subscription Service Stopped");
		}
	}
}