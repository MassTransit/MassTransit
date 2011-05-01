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
	using Magnum.Reflection;
	using SubscriptionBuilders;
	using SubscriptionConfigurators;

	public static class ConsumerSubscriptionConfiguratorExtensions
	{
		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, IConsumerFactory<TConsumer> consumerFactory)
			where TConsumer : class
		{
			var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(consumerFactory);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator;
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator)
			where TConsumer : class, new()
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

			var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator;
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, Func<TConsumer> consumerFactory)
			where TConsumer : class
		{
			var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

			var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator;
		}

		public static ConsumerSubscriptionConfigurator Consumer(this SubscriptionBusServiceConfigurator configurator,
		                                                        Type consumerType,
		                                                        Func<Type, object> consumerFactory)
		{
			var consumerConfigurator =
				(SubscriptionBuilderConfigurator) FastActivator.Create(typeof (UntypedConsumerSubscriptionConfigurator<>),
					new[] {consumerType}, new object[] {consumerFactory});

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return consumerConfigurator as ConsumerSubscriptionConfigurator;
		}
	}
}