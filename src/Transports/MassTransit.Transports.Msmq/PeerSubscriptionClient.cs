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
	using System.Net;
	using Configuration;
	using Internal;
	using log4net;
	using Services.Subscriptions.Client;

	public class PeerSubscriptionClient :
		IBusService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (PeerSubscriptionClient));
		private SubscriptionCoordinator _coordinator;
		private IEndpointResolver _endpointResolver;
		private IServiceBus _subscriptionBus;

		public PeerSubscriptionClient(IEndpointResolver endpointResolver)
		{
			MulticastAddress = new IPEndPoint(IPAddress.Parse("235.109.116.115"), 7784);
			NetworkKey = Environment.MachineName;
			_endpointResolver = endpointResolver;
		}

		public IPEndPoint MulticastAddress { get; set; }

		public string NetworkKey { get; set; }

		public void Dispose()
		{
		}

		public void Start(IServiceBus bus)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting PeerSubscriptionClient on {0}", bus.Endpoint.Uri);

			IEndpointAddress address = bus.ControlBus.Endpoint.Address;

			var baseAddress = new Uri("multicast://" + MulticastAddress + "/");
			var subscriptionAddress = new Uri(baseAddress, address.Uri.AbsolutePath);

			_subscriptionBus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom(subscriptionAddress);
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