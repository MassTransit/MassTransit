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
namespace MassTransit
{
	using System;
	using BusConfigurators;
	using Services.Subscriptions.Configuration;
	using Util;

	public static class SubscriptionClientConfiguratorExtensions
	{
		public static void UseSubscriptionService(this ServiceBusConfigurator configurator, string subscriptionServiceUri)
		{
			UseSubscriptionService(configurator, subscriptionServiceUri.ToUri());
		}

		public static void UseSubscriptionService(this ServiceBusConfigurator configurator, Uri subscriptionServiceUri)
		{
			configurator.ConfigureService<SubscriptionClientConfigurator>(
				x => x.SetSubscriptionServiceEndpoint(subscriptionServiceUri));
		}

		public static void SetSubscriptionServiceEndpoint(this SubscriptionClientConfigurator configurator,
		                                                  string subscriptionServiceUri)
		{
			configurator.SetSubscriptionServiceEndpoint(subscriptionServiceUri.ToUri());
		}
	}
}