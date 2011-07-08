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
	using System.Collections.Generic;
	using System.Linq;
	using Castle.Windsor;
	using Magnum.Extensions;
	using Saga;
	using Saga.SubscriptionConfigurators;
	using SubscriptionConfigurators;
	using WindsorIntegration;

	public static class WindsorContainerExtensions
	{
		public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IWindsorContainer container)
		{
			IList<Type> consumerTypes = FindTypes<IConsumer>(container, x => !x.Implements<ISaga>());
			if (consumerTypes.Count > 0)
			{
				var consumerConfigurator = new WindsorConsumerFactoryConfigurator(configurator, container);

				foreach (Type type in consumerTypes)
				{
					consumerConfigurator.ConfigureConsumer(type);
				}
			}

			IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
			if (sagaTypes.Count > 0)
			{
				var sagaConfigurator = new WindsorSagaFactoryConfigurator(configurator, container);

				foreach (Type type in sagaTypes)
				{
					sagaConfigurator.ConfigureSaga(type);
				}
			}
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, IWindsorContainer container)
			where TConsumer : class
		{
			var consumerFactory = new WindsorConsumerFactory<TConsumer>(container);

			return configurator.Consumer(consumerFactory);
		}

		public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
			this SubscriptionBusServiceConfigurator configurator, IWindsorContainer container)
			where TSaga : class, ISaga
		{
			return configurator.Saga(container.Resolve<ISagaRepository<TSaga>>());
		}

		static IList<Type> FindTypes<T>(IWindsorContainer container, Func<Type, bool> filter)
		{
			return container.Kernel
				.GetAssignableHandlers(typeof (T))
				.Select(h => h.ComponentModel.Implementation)
				.Where(filter)
				.ToList();
		}
	}
}