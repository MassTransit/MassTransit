namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using log4net;
	using Subscriptions;

	/// <summary>
	/// Manages and dispatches messages to correlated message consumers
	/// </summary>
	public class MessageDispatcher :
		IMessageDispatcher
	{
		private static readonly Type _consumes = typeof (Consumes<>.All);
		private static readonly Type _consumesFor = typeof (Consumes<>.For<>);
		private static readonly Type _consumesSelected = typeof (Consumes<>.Selected);
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly Dictionary<Type, IMessageDispatcher> _correlatedDispatchers = new Dictionary<Type, IMessageDispatcher>();
		private readonly object _correlatedLock = new object();
		private readonly ILog _log = LogManager.GetLogger(typeof (MessageDispatcher));
		private readonly Dictionary<Type, IMessageDispatcher> _messageDispatchers = new Dictionary<Type, IMessageDispatcher>();
		private readonly object _messageLock = new object();
		private readonly Dictionary<Type, Type> _messageTypeToKeyType = new Dictionary<Type, Type>();

		public MessageDispatcher()
		{
		}

		public MessageDispatcher(IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_bus = bus;
			_cache = cache;
			_builder = builder;
		}

		#region IMessageDispatcher Members

		public bool Accept(object message)
		{
			bool result = false;

			Type messageType = message.GetType();

			if (_messageTypeToKeyType.ContainsKey(messageType))
			{
				Type keyType = _messageTypeToKeyType[messageType];

				if (_correlatedDispatchers.ContainsKey(keyType))
				{
					result = _correlatedDispatchers[keyType].Accept(message);
				}
			}

			if (_messageDispatchers.ContainsKey(messageType))
			{
				result = _messageDispatchers[messageType].Accept(message);
			}

			return result;
		}

		public void Consume(object message)
		{
			Type messageType = message.GetType();

			if (_messageTypeToKeyType.ContainsKey(messageType))
			{
				Type keyType = _messageTypeToKeyType[messageType];

				if (_correlatedDispatchers.ContainsKey(keyType))
				{
					_correlatedDispatchers[keyType].Consume(message);
				}
			}

			if (_messageDispatchers.ContainsKey(messageType))
			{
				_messageDispatchers[messageType].Consume(message);
			}
		}

		public void Subscribe<T>(T component) where T : class
		{
			Type componentType = typeof (T);

			List<Type> messageTypesMapped = new List<Type>();

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesFor)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					if (messageTypesMapped.Contains(arguments[0]))
						continue;

					IMessageDispatcher dispatcher = GetCorrelatedDispatcher(arguments);

					dispatcher.Subscribe(component);

					messageTypesMapped.Add(arguments[0]);
				}
				else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesSelected)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					if (messageTypesMapped.Contains(arguments[0]))
						continue;

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

					dispatcher.Subscribe(component);

					messageTypesMapped.Add(arguments[0]);
				}
				else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumes)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					if (messageTypesMapped.Contains(arguments[0]))
						continue;

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

					dispatcher.Subscribe(component);

					messageTypesMapped.Add(arguments[0]);
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
				else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumes)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					if (_messageDispatchers.ContainsKey(arguments[0]))
						_messageDispatchers[arguments[0]].Unsubscribe(component);
				}
			}
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			if (_builder == null)
				throw new ArgumentException("No object builder interface is available");

			Type componentType = typeof (TComponent);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesSelected)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

					dispatcher.AddComponent<TComponent>();
				}
				else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumes)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

					dispatcher.AddComponent<TComponent>();
				}
			}
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			if (_builder == null)
				throw new ArgumentException("No object builder interface is available");

			Type componentType = typeof (TComponent);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumesSelected)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

					dispatcher.RemoveComponent<TComponent>();
				}
				else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _consumes)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					IMessageDispatcher dispatcher = GetMessageDispatcher(arguments[0]);

					dispatcher.RemoveComponent<TComponent>();
				}
			}
		}

		public bool Active
		{
			get { return true; }
		}

		#endregion

		public void Dispose()
		{
		}

		private IMessageDispatcher GetCorrelatedDispatcher(Type[] genericArguments)
		{
			lock (_correlatedLock)
			{
				if (_correlatedDispatchers.ContainsKey(genericArguments[1]))
					return _correlatedDispatchers[genericArguments[1]];

				Type dispatcherType = typeof (CorrelationIdDispatcher<,>).MakeGenericType(genericArguments);

				IMessageDispatcher dispatcher = (IMessageDispatcher) Activator.CreateInstance(dispatcherType, _bus, _cache, _builder);

				if (!_messageTypeToKeyType.ContainsKey(genericArguments[0]))
					_messageTypeToKeyType.Add(genericArguments[0], genericArguments[1]);

				_correlatedDispatchers.Add(genericArguments[1], dispatcher);

				return dispatcher;
			}
		}

		private IMessageDispatcher GetMessageDispatcher(Type messageType)
		{
			lock (_messageLock)
			{
				if (_messageDispatchers.ContainsKey(messageType))
					return _messageDispatchers[messageType];

				Type dispatcherType = typeof (MessageDispatcher<>).MakeGenericType(messageType);

				IMessageDispatcher dispatcher = (IMessageDispatcher) Activator.CreateInstance(dispatcherType, _bus, _cache, _builder);

				_messageDispatchers.Add(messageType, dispatcher);

				return dispatcher;
			}
		}
	}

	public class MessageDispatcher<TMessage> :
		IMessageDispatcher where TMessage : class
	{
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly List<Type> _components = new List<Type>();
		private readonly List<Consumes<TMessage>.All> _consumers = new List<Consumes<TMessage>.All>();
		private readonly ILog _log = LogManager.GetLogger(typeof (MessageDispatcher<TMessage>));

		public MessageDispatcher()
		{
		}

		public MessageDispatcher(IServiceBus bus, ISubscriptionCache cache)
		{
			_cache = cache;
			_bus = bus;
		}

		public MessageDispatcher(IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_builder = builder;
			_bus = bus;
			_cache = cache;
		}

		#region IMessageDispatcher Members

		public bool Accept(object obj)
		{
			bool result = false;

			TMessage message = obj as TMessage;
			if (message == null)
				throw new ArgumentException("The message is not of type " + typeof (TMessage).FullName, "obj");

			foreach (Consumes<TMessage>.All consumer in _consumers)
			{
				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					if (selectiveConsumer.Accept(message))
					{
						result = true;
					}
				}
				else
				{
					result = true;
				}
			}

			foreach (Type componentType in _components)
			{
				Consumes<TMessage>.All consumer = _builder.Build<Consumes<TMessage>.All>(componentType);

				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					if (selectiveConsumer.Accept(message))
					{
						result = true;
					}
				}
				else
				{
					result = true;
				}
			}

			return result;
		}

		public void Consume(object obj)
		{
			TMessage message = obj as TMessage;
			if (message == null)
				throw new ArgumentException("The message is not of type " + typeof (TMessage).FullName, "obj");

			foreach (Consumes<TMessage>.All consumer in _consumers)
			{
				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					if (selectiveConsumer.Accept(message))
					{
						consumer.Consume(message);
					}
				}
				else
				{
					consumer.Consume(message);
				}
			}

			foreach (Type componentType in _components)
			{
				Consumes<TMessage>.All consumer = _builder.Build<Consumes<TMessage>.All>(componentType);

				Consumes<TMessage>.Selected selectiveConsumer = consumer as Consumes<TMessage>.Selected;
				if (selectiveConsumer != null)
				{
					if (selectiveConsumer.Accept(message))
					{
						consumer.Consume(message);
					}
				}
				else
				{
					consumer.Consume(message);
				}
			}
		}

		public void Subscribe<TComponent>(TComponent component) where TComponent : class
		{
			Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
			if (consumer == null)
				throw new ArgumentException("The componet does not consume " + typeof (TMessage).FullName, "component");

			if (!_consumers.Contains(consumer))
			{
				_consumers.Add(consumer);
				if (_cache != null)
					_cache.Add(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
			}
		}

		public void Unsubscribe<TComponent>(TComponent component) where TComponent : class
		{
			Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
			if (consumer == null)
				throw new ArgumentException("The componet does not consume " + typeof (TMessage).FullName, "component");

			if (_consumers.Contains(consumer))
			{
				_consumers.Remove(consumer);
				if (_consumers.Count == 0 && _components.Count == 0)
				{
					if (_cache != null)
						_cache.Remove(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
				}
			}
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			if (_builder == null)
				throw new ArgumentException("No builder object was registered");

			if (!_components.Contains(typeof (TComponent)))
			{
				_components.Add(typeof (TComponent));
				if (_cache != null)
					_cache.Add(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
			}
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			if (_builder == null)
				throw new ArgumentException("No builder object was registered");

			if (_components.Contains(typeof (TComponent)))
			{
				_components.Remove(typeof (TComponent));
				if (_consumers.Count == 0 && _components.Count == 0)
				{
					if (_cache != null)
						_cache.Remove(new Subscription(typeof (TMessage).FullName, _bus.Endpoint.Uri));
				}
			}
		}

		public bool Active
		{
			get
			{
				if (_components.Count > 0)
					return true;
				if (_consumers.Count > 0)
					return true;
				return false;
			}
		}

		#endregion

		public void Dispose()
		{
			_consumers.Clear();
			_components.Clear();
		}
	}
}