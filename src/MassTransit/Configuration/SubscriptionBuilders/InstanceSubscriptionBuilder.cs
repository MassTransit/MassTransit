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
namespace MassTransit.SubscriptionBuilders
{
	using System;
	using Magnum.Reflection;
	using Subscriptions;
	using Util;

	public class InstanceSubscriptionBuilder :
		SubscriptionBuilder
	{
		readonly object _instance;
		readonly Func<UnsubscribeAction, ISubscriptionReference> _referenceFactory;

		public InstanceSubscriptionBuilder(object instance,
		                                   Func<UnsubscribeAction, ISubscriptionReference> referenceFactory)
		{
			_instance = instance;
			_referenceFactory = referenceFactory;
		}

		public ISubscriptionReference Subscribe(IServiceBus bus)
		{
			Type instanceType = _instance.GetType();
			var genericTypes = new[] {instanceType};

			UnsubscribeAction unsubscribe = this.FastInvoke<InstanceSubscriptionBuilder, UnsubscribeAction>(genericTypes,
				"AddSubscription", bus, _instance);

			return _referenceFactory(unsubscribe);
		}

		[UsedImplicitly]
		UnsubscribeAction AddSubscription<T>(IServiceBus bus, T instance)
			where T : class
		{
			return bus.Subscribe(instance);
		}
	}
}