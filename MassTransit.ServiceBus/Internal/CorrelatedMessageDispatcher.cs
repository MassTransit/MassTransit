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
		private static readonly Type _consumesFor = typeof (Consumes<>.For<>);
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

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesFor)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetDispatcher(arguments);

					dispatcher.Subscribe(component);
				}
			}
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			Type componentType = typeof (T);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesFor)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					if (_dispatchers.ContainsKey(arguments[1]))
						_dispatchers[arguments[1]].Unsubscribe(component);
				}
			}
		}

		private IMessageDispatcher GetDispatcher(Type[] genericArguments)
		{
			if (_dispatchers.ContainsKey(genericArguments[1]))
				return _dispatchers[genericArguments[1]];

			Type dispatcherType = typeof (CorrelationIdDispatcher<,>).MakeGenericType(genericArguments);

			IMessageDispatcher dispatcher = (IMessageDispatcher) Activator.CreateInstance(dispatcherType);

			if (!_messageTypeToKeyType.ContainsKey(genericArguments[0]))
				_messageTypeToKeyType.Add(genericArguments[0], genericArguments[1]);

			_dispatchers.Add(genericArguments[1], dispatcher);

			return dispatcher;
		}
	}
}