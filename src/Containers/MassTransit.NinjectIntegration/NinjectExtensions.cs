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
	using Magnum.Extensions;
	using Ninject;
	using NinjectIntegration;
	using Saga;
	using Saga.SubscriptionConfigurators;
	using SubscriptionConfigurators;

	public static class NinjectExtensions
	{
		public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IKernel kernel)
		{
			// Note that this might not be the right thing to do here, since they aren't registering
			// the consumer and there is no way to enumerate all the bindings in NInject

			IList<Type> concreteTypes = FindTypes<IConsumer>(kernel, x => !x.Implements<ISaga>());

			if (concreteTypes.Count != 0)
			{
				foreach (Type type in concreteTypes)
				{
					configurator.Consumer(type, t => kernel.Get(t));
				}
			}

			IList<Type> sagaTypes = FindTypes<ISaga>(kernel, x => true);
			if (sagaTypes.Count > 0)
			{
				var sagaConfigurator = new NinjectSagaFactoryConfigurator(configurator, kernel);

				foreach (Type type in sagaTypes)
				{
					sagaConfigurator.ConfigureSaga(type);
				}
			}
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, IKernel kernel)
			where TConsumer : class
		{
			var consumerFactory = new NinjectConsumerFactory<TConsumer>(kernel);

			return configurator.Consumer(consumerFactory);
		}

		public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
			this SubscriptionBusServiceConfigurator configurator, IKernel kernel)
			where TSaga : class, ISaga
		{
			return configurator.Saga(kernel.Get<ISagaRepository<TSaga>>());
		}

		private static IList<Type> FindTypes<T>(IKernel kernel, Func<Type, bool> filter)
		{
			return kernel.GetBindings(typeof (T))
				.Select(x => x.Service)
				.Distinct()
				.Where(filter)
				.ToList();
		}
	}
}