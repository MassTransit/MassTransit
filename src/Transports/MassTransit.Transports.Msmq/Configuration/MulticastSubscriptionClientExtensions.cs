// Copyright 2007-2013 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using Transports.Msmq.Configuration;

	public static class MulticastSubscriptionClientExtensions
	{
        [Obsolete("The extension method on UseMsmq should be used instead")]
        public static void UseMulticastSubscriptionClient(this ServiceBusConfigurator configurator)
		{
			UseMulticastSubscriptionClient(configurator, x => { });
		}

        public static void UseMulticastSubscriptionClient(this MsmqConfigurator configurator)
		{
			UseMulticastSubscriptionClient(configurator, x => { });
		}

        [Obsolete("The extension method on UseMsmq should be used instead")]
        public static void UseMulticastSubscriptionClient(this ServiceBusConfigurator configurator,
		                                                  Action<MulticastSubscriptionClientConfigurator> configureCallback)
		{
			var clientConfigurator = new MulticastSubscriptionClientConfiguratorImpl();

			configureCallback(clientConfigurator);

			configurator.AddSubscriptionObserver(clientConfigurator.Create);
		}

        public static void UseMulticastSubscriptionClient(this MsmqConfigurator configurator,
		                                                  Action<MulticastSubscriptionClientConfigurator> configureCallback)
		{
			var clientConfigurator = new MulticastSubscriptionClientConfiguratorImpl();

			configureCallback(clientConfigurator);

			configurator.Configurator.AddSubscriptionObserver(clientConfigurator.Create);
		}
	}
}