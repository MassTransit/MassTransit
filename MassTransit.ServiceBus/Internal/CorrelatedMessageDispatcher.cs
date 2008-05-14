namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Manages and dispatches messages to correlated message consumers
	/// </summary>
	public class CorrelatedMessageDispatcher :
		IMessageDispatcher
	{
		private readonly Dictionary<Type, IMessageDispatcher> _dispatchers = new Dictionary<Type, IMessageDispatcher>();
		private readonly Dictionary<Type, Type> _messageTypeToKeyType = new Dictionary<Type, Type>();

		public bool Dispatch(object message)
		{
			Type messageType = message.GetType();

			if (_messageTypeToKeyType.ContainsKey(messageType))
			{
				Type keyType = _messageTypeToKeyType[messageType];

				if (_dispatchers.ContainsKey(keyType))
				{
					return _dispatchers[keyType].Dispatch(message);
				}
			}

			return false;
		}

		public void Subscribe<T>(T component) where T : class
		{
			Type componentType = typeof (T);

			Type correlatedType = typeof (Consumes<>.For<>);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == correlatedType)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					Type dispatcherType = typeof (CorrelationIdDispatcher<,>).MakeGenericType(arguments);

					IMessageDispatcher dispatcher = (IMessageDispatcher) Activator.CreateInstance(dispatcherType);

					_messageTypeToKeyType.Add(arguments[0], arguments[1]);
					_dispatchers.Add(arguments[1], dispatcher);

					dispatcher.Subscribe(component);
				}
			}
		}
	}
}