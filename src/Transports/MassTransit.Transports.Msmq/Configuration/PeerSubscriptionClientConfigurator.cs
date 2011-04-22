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
	using Transports.Msmq;

	public class PeerSubscriptionClientConfigurator :
		IPeerSubscriptionClientConfigurator,
		IServiceConfigurator
	{
		private IPEndPoint _multicastAddress;

		public PeerSubscriptionClientConfigurator()
		{
			_multicastAddress = new IPEndPoint(IPAddress.Parse("235.109.116.115"), 7784);
		}

		public Type ServiceType
		{
			get { return typeof (PeerSubscriptionClient); }
		}

		public IBusService Create(IServiceBus bus, IObjectBuilder builder)
		{
			var endpointFactory = builder.GetInstance<IEndpointResolver>();

			var service = new PeerSubscriptionClient(endpointFactory)
				{
					MulticastAddress = _multicastAddress
				};

			return service;
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