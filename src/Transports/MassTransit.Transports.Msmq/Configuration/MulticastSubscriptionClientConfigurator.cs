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
namespace MassTransit.Transports.Msmq.Configuration
{
	using System;
	using System.Net;
	using Builders;
	using BusConfigurators;
	using BusServiceConfigurators;
	using Exceptions;
	using Magnum;
	using Magnum.Extensions;
	using Util;

	public class MulticastSubscriptionClientConfigurator :
		IMulticastSubscriptionClientConfigurator,
		BusServiceConfigurator
	{
		IPEndPoint _multicastAddress;
		string _networkKey;

		public MulticastSubscriptionClientConfigurator()
		{
			_multicastAddress = new IPEndPoint(IPAddress.Parse("235.109.116.115"), 7784);
			_networkKey = Environment.MachineName.ToLowerInvariant();
		}

		public Type ServiceType
		{
			get { return typeof (MulticastSubscriptionClient); }
		}

		public IBusService Create(IServiceBus bus)
		{
			string path = bus.ControlBus.Endpoint.Address.Uri.AbsolutePath;

			Uri uri = new UriBuilder("msmq-pgm", _multicastAddress.Address.ToString(), _multicastAddress.Port, path).Uri;
			Uri clientUri = uri.AppendToPath("_subscriptions");

			var builder = new ControlBusBuilderImpl(new ServiceBusSettings
				{
					ConcurrentConsumerLimit = 1,
					ConcurrentReceiverLimit = 1,
					// REVIEW get rid of this damn thing (what thing, I don't get it, if Dru doesn't than remove this todo)
					AutoStart = true,
					EndpointCache = bus.EndpointCache,
					InputAddress = clientUri,
					ReceiveTimeout = 3.Seconds(),
				});

			IControlBus subscriptionBus = builder.Build();

			var service = new MulticastSubscriptionClient(subscriptionBus, clientUri, _networkKey);

			return service;
		}

		public void SetNetworkKey(string key)
		{
			Guard.AgainstEmpty(key, "key");

			_networkKey = key;
		}

		public void SetMulticastAddress(string uriString)
		{
			try
			{
				var uri = new Uri(uriString.ToLowerInvariant());

				var ipEndPoint = new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port);

				_multicastAddress = ipEndPoint;
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The multicast address must match the format: msmq-pgm://x.x.x.x:port/ ", ex);
			}
		}
	}
}