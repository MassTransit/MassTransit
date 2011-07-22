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
	using Pipeline;
	using SubscriptionBuilders;
	using SubscriptionConfigurators;
	using SubscriptionConnectors;

	public static class HandlerSubscriptionExtensions
	{
		/// <summary>
		/// Subscribes a message handler (which can be any delegate of the message type,
		/// such as a class instance method, a delegate, or a lambda expression)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="configurator"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static HandlerSubscriptionConfigurator<T> Handler<T>(this SubscriptionBusServiceConfigurator configurator,
		                                                            Action<T> handler)
			where T : class
		{
			var handlerConfigurator = new HandlerSubscriptionConfiguratorImpl<T>(handler);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(handlerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return handlerConfigurator;
		}

		public static HandlerSubscriptionConfigurator<T> Handler<T>(this SubscriptionBusServiceConfigurator configurator,
		                                                            Action<IConsumeContext<T>, T> handler)
			where T : class
		{
			var handlerConfigurator = new HandlerSubscriptionConfiguratorImpl<T>(handler);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(handlerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return handlerConfigurator;
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="bus"></param>
		/// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public static UnsubscribeAction SubscribeHandler<T>(this IServiceBus bus, Action<T> handler)
			where T : class
		{
			return SubscribeHandlerSelector(bus, HandlerSelector.ForHandler(handler));
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="bus"></param>
		/// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public static UnsubscribeAction SubscribeContextHandler<T>(this IServiceBus bus, Action<IConsumeContext<T>> handler)
			where T : class
		{
			return SubscribeHandlerSelector(bus, HandlerSelector.ForContextHandler(handler));
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
			return SubscribeHandlerSelector(bus, HandlerSelector.ForSelectiveHandler(condition, handler));
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="bus"></param>
		/// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public static UnsubscribeAction SubscribeHandlerSelector<T>(this IServiceBus bus, HandlerSelector<T> handler)
			where T : class
		{
			var connector = new HandlerSubscriptionConnector<T>();

			return bus.Configure(x => connector.Connect(x, handler));
		}
	}
}