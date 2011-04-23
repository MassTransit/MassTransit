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
namespace MassTransit.Configuration
{
	using System;
	using System.Net;
	using Exceptions;
	using Internal;
	using Magnum;
	using Transports.Msmq;

	public class MulticastSubscriptionClientConfigurator :
		IMulticastSubscriptionClientConfigurator,
		IServiceConfigurator
	{
		private IPEndPoint _multicastAddress;
		private string _networkKey;

		public MulticastSubscriptionClientConfigurator()
		{
			_multicastAddress = new IPEndPoint(IPAddress.Parse("235.109.116.115"), 7784);
			_networkKey = Environment.MachineName.ToLowerInvariant();
		}

		public Type ServiceType
		{
			get { return typeof (MulticastSubscriptionClient); }
		}

		public IBusService Create(IServiceBus bus, IObjectBuilder builder)
		{
			string path = bus.ControlBus.Endpoint.Address.Uri.AbsolutePath;

			var uri = new UriBuilder("msmq-pgm", _multicastAddress.Address.ToString(), _multicastAddress.Port, path).Uri;
			Uri clientUri = uri.AppendToPath("_subscriptions");

			var service = new MulticastSubscriptionClient(clientUri, _networkKey, builder);

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