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
using Magnum.Pipeline;

namespace MassTransit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
    using Magnum.Extensions;
    using Microsoft.Practices.Unity;
    using SubscriptionConfigurators;
	using UnityIntegration;

	public static class UnityExtensions
	{
		public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IUnityContainer container)
		{
			IList<Type> concreteTypes = container.Registrations
				.Where(r => r.MappedToType.Implements<IConsumer>())
				.Select(r => r.MappedToType)
				.ToList();

			if (concreteTypes.Count == 0)
				return;

			var consumerConfigurator = new UnityConsumerFactoryConfigurator(configurator, container);

			foreach (Type concreteType in concreteTypes)
			{
				consumerConfigurator.ConfigureConsumer(concreteType);
			}
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, IUnityContainer container)
			where TConsumer : class
		{
			var consumerFactory = new UnityConsumerFactory<TConsumer>(container);

			return configurator.Consumer(consumerFactory);
		}
	}
}