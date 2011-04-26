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
	using SubscriptionConfigurators;

	public static class SubscriptionConfiguratorExtensions
	{
		public static void RegisterSubscription(this ServiceBusConfigurator configurator, Action<SubscriptionConfigurator> configure)
		{
			var subscriptionConfigurator = new SubscriptionConfiguratorImpl(configurator);

			configure(subscriptionConfigurator);
		}

		/// <summary>
		/// Subscribes an object instance that implements one or more Consumes&lt;T&gt;.* interfaces
		/// to receive messages
		/// TODO: DRU/SUB
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="consumerInstance">The instance of the consumer object</param>
		public static void AddConsumerInstance(this SubscriptionConfigurator configurator, object consumerInstance)
		{
			// build up the model class for this consumer, and add it to the configurator
		}

		/// <summary>
		/// Adds a consumer type, and specifies the factory method to create the consumer to handle the
		/// message (which can delegate to the container, or constructor call).
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="consumerType">The type of object being registered</param>
		/// <param name="objectFactory">The factory method is called once for every message that is received.</param>
		public static void RegisterConsumer(this SubscriptionConfigurator configurator, Type consumerType, Func<Type, object> objectFactory)
		{

		}
	}
}