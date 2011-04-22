// Copyright 2007-2011 The Apache Software Foundation.
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
	using Configuration;
	using Internal;
	using log4net;
	using Services.Subscriptions.Client;

	public class MulticastSubscriptionClient :
		IBusService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MulticastSubscriptionClient));
		private SubscriptionCoordinator _coordinator;
		private readonly Uri _uri;
		private IEndpointResolver _endpointResolver;
		private IServiceBus _subscriptionBus;

		public MulticastSubscriptionClient(Uri uri, string networkKey, IEndpointResolver endpointResolver)
		{
			_uri = uri;

			NetworkKey = networkKey;
			_endpointResolver = endpointResolver;
		}

		public string NetworkKey { get; private set; }

		public void Dispose()
		{
		}

		public void Start(IServiceBus bus)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting MulticastSubscriptionClient on {0}", _uri);

			_subscriptionBus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom(_uri);
					x.SetEndpointFactory(_endpointResolver);
					x.SetConcurrentConsumerLimit(1);
				});

			_coordinator = new SubscriptionCoordinator(_subscriptionBus, _subscriptionBus.Endpoint, _endpointResolver);
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
	}
}