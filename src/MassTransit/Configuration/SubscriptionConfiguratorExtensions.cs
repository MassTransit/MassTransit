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
	using Saga;
	using SubscriptionBuilders;
	using SubscriptionConfigurators;
	using Util;

	public static class SubscriptionConfiguratorExtensions
	{
		public static void Subscribe(this ServiceBusConfigurator configurator,
		                             Action<SubscriptionBusServiceConfigurator> configure)
		{
			var subscriptionConfigurator = new SubscriptionBusServiceConfiguratorImpl();

			configure(subscriptionConfigurator);

			configurator.AddBusConfigurator(subscriptionConfigurator);
		}

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

		/// <summary>
		/// Subscribes an object instance to the bus
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static InstanceSubscriptionConfigurator Instance(this SubscriptionBusServiceConfigurator configurator,
		                                                        object instance)
		{
			var instanceConfigurator = new InstanceSubscriptionConfiguratorImpl(instance);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(instanceConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return instanceConfigurator;
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, Func<Action<Action<TConsumer>>> consumerFactory)
			where TConsumer : class
		{
			var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(consumerFactory);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator;
		}

		public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
			this SubscriptionBusServiceConfigurator configurator, ISagaRepository<TSaga> sagaRepository)
			where TSaga : class, ISaga
		{
			var sagaConfigurator = new SagaSubscriptionConfiguratorImpl<TSaga>(sagaRepository);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(sagaConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return sagaConfigurator;
		}

		public static void Consumer(this SubscriptionBusServiceConfigurator configurator, Type consumerType,
		                            Func<Type, object> consumerFactory)
		{
			throw new NotImplementedException("The consumer by type has not yet been implemented.");
		}
	}
}