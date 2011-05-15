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
	using Configuration;
	using Magnum.Reflection;
	using SubscriptionBuilders;
	using SubscriptionConfigurators;
	using SubscriptionConnectors;
	using Util;

	public static class ConsumerSubscriptionExtensions
	{
		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			[NotNull] this SubscriptionBusServiceConfigurator configurator, [NotNull] IConsumerFactory<TConsumer> consumerFactory)
			where TConsumer : class
		{
			var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(consumerFactory);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator;
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			[NotNull] this SubscriptionBusServiceConfigurator configurator)
			where TConsumer : class, new()
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

			var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator;
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			[NotNull] this SubscriptionBusServiceConfigurator configurator, [NotNull] Func<TConsumer> consumerFactory)
			where TConsumer : class
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

			var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator;
		}

		public static ConsumerSubscriptionConfigurator Consumer(
			[NotNull] this SubscriptionBusServiceConfigurator configurator,
			[NotNull] Type consumerType,
			[NotNull] Func<Type, object> consumerFactory)
		{
			var consumerConfigurator =
				(SubscriptionBuilderConfigurator) FastActivator.Create(typeof (UntypedConsumerSubscriptionConfigurator<>),
					new[] {consumerType}, new object[] {consumerFactory});

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator as ConsumerSubscriptionConfigurator;
		}

		public static UnsubscribeAction SubscribeConsumer<TConsumer>([NotNull] this IServiceBus bus)
			where TConsumer : class, new()
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

			ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector(delegateConsumerFactory);

			return bus.Configure(connector.Connect);
		}

		public static UnsubscribeAction SubscribeConsumer<TConsumer>([NotNull] this IServiceBus bus,
		                                                             [NotNull] Func<TConsumer> consumerFactory)
			where TConsumer : class
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

			ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector(delegateConsumerFactory);

			return bus.Configure(connector.Connect);
		}

		public static UnsubscribeAction SubscribeConsumer([NotNull] this IServiceBus bus, [NotNull] Type consumerType,
		                                                  [NotNull] Func<Type, object> consumerFactory)
		{
			ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector(consumerType, consumerFactory);

			return bus.Configure(connector.Connect);
		}
	}
}