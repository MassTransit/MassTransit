/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using Subscriptions;

	public class SubscriptionCoordinator
	{
		private static readonly Type _consumerType = typeof (Consumes<>.All);
		private static readonly Type _correlatedConsumerType = typeof (Consumes<>.For<>);
		private static readonly Type _selectiveConsumerType = typeof (Consumes<>.Selected);
		private readonly Dictionary<Type, SubscriptionTypeInfo> _types = new Dictionary<Type, SubscriptionTypeInfo>();
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly IMessageTypeDispatcher _dispatcher;

		public SubscriptionCoordinator(IMessageTypeDispatcher dispatcher, IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_dispatcher = dispatcher;
			_bus = bus;
			_cache = cache;
			_builder = builder;
		}

		public ISubscriptionTypeInfo Resolve<TComponent>()
		{
			return Resolve(typeof (TComponent));
		}

		public ISubscriptionTypeInfo Resolve<TComponent>(TComponent component)
		{
			return Resolve(typeof (TComponent));
		}

		public ISubscriptionTypeInfo Resolve(Type componentType)
		{
			lock (_types)
			{
				SubscriptionTypeInfo info;
				if (_types.TryGetValue(componentType, out info))
					return info;

				info = new SubscriptionTypeInfo();

				List<Type> usedMessageTypes = new List<Type>();

				foreach (Type interfaceType in componentType.GetInterfaces())
				{
					if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _correlatedConsumerType)
					{
						Type[] arguments = interfaceType.GetGenericArguments();

						if (usedMessageTypes.Contains(arguments[0]))
							continue;

						Type subscriptionType = typeof (CorrelatedSubscription<,,>).MakeGenericType(componentType, arguments[0], arguments[1]);

						ISubscriptionTypeInfo subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType, _dispatcher, _bus, _cache, _builder);

						info.Add(subscriptionTypeInfo);

						usedMessageTypes.Add(arguments[0]);
					}
					else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _selectiveConsumerType)
					{
						Type[] arguments = interfaceType.GetGenericArguments();

						if (usedMessageTypes.Contains(arguments[0]))
							continue;

						Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, arguments[0]);

						ISubscriptionTypeInfo subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType, SubscriptionMode.Selected, _dispatcher, _bus, _cache, _builder);

						info.Add(subscriptionTypeInfo);

						usedMessageTypes.Add(arguments[0]);
					}
					else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumerType)
					{
						Type[] arguments = interfaceType.GetGenericArguments();

						if (usedMessageTypes.Contains(arguments[0]))
							continue;

						Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, arguments[0]);

						ISubscriptionTypeInfo subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType, SubscriptionMode.All, _dispatcher, _bus, _cache, _builder);

						info.Add(subscriptionTypeInfo);

						usedMessageTypes.Add(arguments[0]);
					}
				}

				_types.Add(componentType, info);

				return info;
			}
		}
	}

	public enum SubscriptionMode
	{
		All,
		Selected,
		Correlated,
	}
}