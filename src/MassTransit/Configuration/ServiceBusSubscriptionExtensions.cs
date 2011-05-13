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
	using Magnum;
	using SubscriptionConnectors;
	using Util;

	public static class ServiceBusSubscriptionExtensions
	{
		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="bus"></param>
		/// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public static UnsubscribeAction SubscribeHandler<T>(this IServiceBus bus, Action<T> handler)
			where T : class
		{
			return SubscribeSelectiveHandler<T>(bus, message => handler);
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="bus"></param>
		/// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
		/// <param name="condition"></param>
		public static UnsubscribeAction SubscribeHandler<T>(this IServiceBus bus, Action<T> handler, Predicate<T> condition)
			where T : class
		{
			return SubscribeSelectiveHandler<T>(bus, message => condition(message) ? handler : null);
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="bus"></param>
		/// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public static UnsubscribeAction SubscribeSelectiveHandler<T>(this IServiceBus bus, Func<T, Action<T>> handler)
			where T : class
		{
			var connector = new HandlerSubscriptionConnector<T>();

			return bus.Configure(x => connector.Connect(x, handler));
		}

		/// <summary>
		/// Connects any consumers for the component to the message dispatcher
		/// </summary>
		/// <param name="bus"></param>
		/// <param name="instance"></param>
		public static UnsubscribeAction SubscribeInstance(this IServiceBus bus, object instance)
		{
			Guard.AgainstNull(instance, "instance", "A null instance cannot be subscribed");

			InstanceConnector connector = InstanceConnectorCache.GetInstanceConnector(instance.GetType());

			return bus.Configure(x => connector.Connect(x, instance));
		}

		/// <summary>
		/// Connects any consumers for the component to the message dispatcher
		/// </summary>
		/// <typeparam name="T">The consumer type</typeparam>
		/// <param name="bus"></param>
		/// <param name="instance"></param>
		public static UnsubscribeAction SubscribeInstance<T>(this IServiceBus bus, T instance)
			where T : class
		{
			Guard.AgainstNull(instance, "instance", "A null instance cannot be subscribed");

			InstanceConnector connector = InstanceConnectorCache.GetInstanceConnector<T>();

			return bus.Configure(x => connector.Connect(x, instance));
		}

		public static UnsubscribeAction SubscribeConsumer<TConsumer>(this IServiceBus bus)
			where TConsumer : class, new()
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

			ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector(delegateConsumerFactory);

			return bus.Configure(connector.Connect);
		}

		public static UnsubscribeAction SubscribeConsumer<TConsumer>(this IServiceBus bus, Func<TConsumer> consumerFactory)
			where TConsumer : class
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

			ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector(delegateConsumerFactory);

			return bus.Configure(connector.Connect);
		}

		public static UnsubscribeAction SubscribeConsumer(this IServiceBus bus, Type consumerType,
		                                                  Func<Type, object> consumerFactory)
		{
			ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector(consumerType, consumerFactory);

			return bus.Configure(connector.Connect);
		}
	}
}