namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;

	public class SubscriptionTypeInfo
	{
		private static readonly Type _consumerType = typeof (Consumes<>.All);
		private static readonly Type _correlatedConsumerType = typeof (Consumes<>.For<>);
		private static readonly Type _selectiveConsumerType = typeof (Consumes<>.Selected);
		private static readonly Dictionary<Type, SubscriptionTypeInfo> _types = new Dictionary<Type, SubscriptionTypeInfo>();

		private readonly List<IMessageTypeSubscription> _messageTypeSubscriptions = new List<IMessageTypeSubscription>();

		private void Add(IMessageTypeSubscription subscription)
		{
			_messageTypeSubscriptions.Add(subscription);
		}

		public void Subscribe<T>(MessageTypeDispatcher dispatcher, T component) where T : class
		{
			foreach (IMessageTypeSubscription subscription in _messageTypeSubscriptions)
			{
				subscription.Subscribe(dispatcher, component);
			}
		}

		public void Unsubscribe<T>(MessageTypeDispatcher dispatcher, T component) where T : class
		{
			foreach (IMessageTypeSubscription subscription in _messageTypeSubscriptions)
			{
				subscription.Unsubscribe<T>(dispatcher, component);
			}
		}

		public void AddComponent(MessageTypeDispatcher dispatcher)
		{
			foreach (IMessageTypeSubscription subscription in _messageTypeSubscriptions)
			{
				subscription.AddComponent(dispatcher);
			}
		}

		public void RemoveComponent(MessageTypeDispatcher dispatcher)
		{
			foreach (IMessageTypeSubscription subscription in _messageTypeSubscriptions)
			{
				subscription.AddComponent(dispatcher);
			}
		}

		public static SubscriptionTypeInfo Resolve(Type componentType, IObjectBuilder builder)
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

						Type subscriptionType = typeof(CorrelatedSubscription<,,>).MakeGenericType(componentType, arguments[0], arguments[1]);

						IMessageTypeSubscription subscription = (IMessageTypeSubscription)Activator.CreateInstance(subscriptionType, builder);

						info.Add(subscription);

						usedMessageTypes.Add(arguments[0]);
					}
					else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _selectiveConsumerType)
					{
						Type[] arguments = interfaceType.GetGenericArguments();

						if (usedMessageTypes.Contains(arguments[0]))
							continue;

						Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, arguments[0]);

						IMessageTypeSubscription subscription = (IMessageTypeSubscription) Activator.CreateInstance(subscriptionType, SubscriptionMode.Selected, builder);

						info.Add(subscription);

						usedMessageTypes.Add(arguments[0]);
					}
					else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumerType)
					{
						Type[] arguments = interfaceType.GetGenericArguments();

						if (usedMessageTypes.Contains(arguments[0]))
							continue;

						Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, arguments[0]);

						IMessageTypeSubscription subscription = (IMessageTypeSubscription) Activator.CreateInstance(subscriptionType, SubscriptionMode.All, builder);

						info.Add(subscription);

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