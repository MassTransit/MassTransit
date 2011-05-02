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
namespace MassTransit.NinjectIntegration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ninject;
	using SubscriptionConfigurators;

	public static class MassTransitNinjectExtensions
	{
		public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IKernel kernel)
		{
			// Note that this might not be the right thing to do here, since they aren't registering
			// the consumer and there is no way to enumerate all the bindings in NInject

			IList<Type> concreteTypes = kernel.GetBindings(typeof (Consumes<>.All))
				.Select(x => x.Service)
				.ToList();

			if (concreteTypes.Count == 0)
				return;

			foreach (Type type in concreteTypes)
			{
				configurator.Consumer(type, t => kernel.Get(t));
			}
		}
	}
}