// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Services.Subscriptions.Configuration
{
	using System;
	using Client;
	using Exceptions;
	using Internal;
	using MassTransit.Configuration;

	public class SubscriptionClientConfigurator :
		IServiceConfigurator
	{
		private Uri _subscriptionServiceUri;

		public Type ServiceType
		{
			get { return typeof (SubscriptionClient); }
		}

		public IBusService Create(IServiceBus bus, IObjectBuilder builder)
		{
			var endpointFactory = builder.GetInstance<IEndpointFactory>();

			var service = new SubscriptionClient(endpointFactory)
				{
					SubscriptionServiceUri = _subscriptionServiceUri
				};

			return service;
		}

		public void SetSubscriptionServiceEndpoint(string uriString)
		{
			try
			{
				Uri uri = new Uri(uriString.ToLowerInvariant());

				_subscriptionServiceUri = uri;
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The endpoint Uri is invalid: " + uriString, ex);
			}
		}

		public void SetSubscriptionServiceEndpoint(Uri uri)
		{
			_subscriptionServiceUri = uri;
		}
	}
}