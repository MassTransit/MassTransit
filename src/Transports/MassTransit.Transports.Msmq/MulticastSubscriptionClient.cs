// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.Msmq
{
	using System;
	using log4net;
	using Services.Subscriptions.Client;

	public class MulticastSubscriptionClient :
		IBusService
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (MulticastSubscriptionClient));
		readonly string _networkKey;
		readonly Uri _uri;
		SubscriptionCoordinator _coordinator;
		bool _disposed;
		IServiceBus _subscriptionBus;

		public MulticastSubscriptionClient(IServiceBus subscriptionBus, Uri uri, string networkKey)
		{
			_subscriptionBus = subscriptionBus;
			_uri = uri;

			_networkKey = networkKey;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start(IServiceBus bus)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting MulticastSubscriptionClient on {0}", _uri);

			_coordinator = new SubscriptionCoordinator(_subscriptionBus, _subscriptionBus.Endpoint, _networkKey, true);
			_coordinator.Start(bus);
		}

		public void Stop()
		{
			_coordinator.Stop();
			_coordinator.Dispose();
			_coordinator = null;

			_subscriptionBus.Dispose();
			_subscriptionBus = null;
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_coordinator != null)
				{
					_coordinator.Dispose();
					_coordinator = null;
				}

				if (_subscriptionBus != null)
				{
					_subscriptionBus.Dispose();
					_subscriptionBus = null;
				}
			}

			_disposed = true;
		}

		~MulticastSubscriptionClient()
		{
			Dispose(false);
		}
	}
}