namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Manages and dispatches messages to correlated message consumers
	/// </summary>
	public class MessageDispatcher :
		IMessageDispatcher
	{
		private static readonly Type _consumes = typeof (Consumes<>.Any);
		private static readonly Type _consumesSelected = typeof (Consumes<>.Selected);
		private static readonly Type _consumesFor = typeof (Consumes<>.For<>);
		private readonly Dictionary<Type, IMessageDispatcher> _correlatedDispatchers = new Dictionary<Type, IMessageDispatcher>();
		private readonly Dictionary<Type, IMessageDispatcher> _messageDispatchers = new Dictionary<Type, IMessageDispatcher>();
		private readonly Dictionary<Type, Type> _messageTypeToKeyType = new Dictionary<Type, Type>();

		public bool Dispatch(object message)
		{
			bool result = false;

			Type messageType = message.GetType();

			if (_messageTypeToKeyType.ContainsKey(messageType))
			{
				Type keyType = _messageTypeToKeyType[messageType];

				if (_correlatedDispatchers.ContainsKey(keyType))
				{
					result = _correlatedDispatchers[keyType].Dispatch(message);
				}
			}

			if (_messageDispatchers.ContainsKey(messageType))
			{
				result = _messageDispatchers[messageType].Dispatch(message);
			}

			return result;
		}

		public void Subscribe<T>(T component) where T : class
		{
			Type componentType = typeof (T);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesFor)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetCorrelatedDispatcher(arguments);

					dispatcher.Subscribe(component);
				}
				else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesSelected)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

					dispatcher.Subscribe(component);
				}
				else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumes)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

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

					if (_correlatedDispatchers.ContainsKey(arguments[1]))
						_correlatedDispatchers[arguments[1]].Unsubscribe(component);
				}
			}
		}

		private IMessageDispatcher GetCorrelatedDispatcher(Type[] genericArguments)
		{
			if (_correlatedDispatchers.ContainsKey(genericArguments[1]))
				return _correlatedDispatchers[genericArguments[1]];

			Type dispatcherType = typeof (CorrelationIdDispatcher<,>).MakeGenericType(genericArguments);

			IMessageDispatcher dispatcher = (IMessageDispatcher) Activator.CreateInstance(dispatcherType);

			if (!_messageTypeToKeyType.ContainsKey(genericArguments[0]))
				_messageTypeToKeyType.Add(genericArguments[0], genericArguments[1]);

			_correlatedDispatchers.Add(genericArguments[1], dispatcher);

			return dispatcher;
		}

		private IMessageDispatcher GetMessageDispatcher(Type messageType)
		{
			if (_messageDispatchers.ContainsKey(messageType))
				return _messageDispatchers[messageType];

			Type dispatcherType = typeof (MessageDispatcher<>).MakeGenericType(messageType);

			IMessageDispatcher dispatcher = (IMessageDispatcher) Activator.CreateInstance(dispatcherType);

			_messageDispatchers.Add(messageType, dispatcher);

			return dispatcher;
		}
	}

	public class MessageDispatcher<TMessage> :
		IMessageDispatcher where TMessage : class
	{
		private readonly List<Consumes<TMessage>.Any> _consumers = new List<Consumes<TMessage>.Any>();

		public bool Dispatch(object obj)
		{
			bool result = false;

			TMessage message = obj as TMessage;
			if (message == null)
				throw new ArgumentException("The message is not of type " + typeof (TMessage).FullName, "obj");

			foreach (Consumes<TMessage>.Any consumer in _consumers)
			{
				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					if (selectiveConsumer.Accept(message))
					{
						result = true;
						consumer.Consume(message);
					}
				}
				else
				{
					result = true;
					consumer.Consume(message);
				}
			}

			return result;
		}

		public void Subscribe<TComponent>(TComponent component) where TComponent : class
		{
			Consumes<TMessage>.Any consumer = component as Consumes<TMessage>.Any;
			if (consumer == null)
				throw new ArgumentException("The componet does not consume " + typeof (TMessage).FullName, "component");

			if (!_consumers.Contains(consumer))
				_consumers.Add(consumer);
		}

		public void Unsubscribe<TComponent>(TComponent component) where TComponent : class
		{
			Consumes<TMessage>.Any consumer = component as Consumes<TMessage>.Any;
			if (consumer == null)
				throw new ArgumentException("The componet does not consume " + typeof (TMessage).FullName, "component");

			if (_consumers.Contains(consumer))
				_consumers.Remove(consumer);
		}
	}
}