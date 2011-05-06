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
	using Autofac;
	using AutofacIntegration;
	using Magnum.Extensions;
	using SubscriptionConfigurators;

	public static class AutofacExtensions
	{
		public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IContainer container)
		{
			IList<Type> concreteTypes = container.ComponentRegistry.Registrations
				.Where(r => r.Services.Any(s => s.Implements<IConsumer>()))
				.Select(r => r.Activator.LimitType)
				.ToList();

			if (concreteTypes.Count == 0)
				return;

			var consumerConfigurator = new AutofacConsumerFactoryConfigurator(configurator, container);

			foreach (Type concreteType in concreteTypes)
			{
				consumerConfigurator.ConfigureConsumer(concreteType);
			}
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, IContainer kernel)
			where TConsumer : class
		{
			var consumerFactory = new AutofacConsumerFactory<TConsumer>(kernel);

			return configurator.Consumer(consumerFactory);
		}
	}
}