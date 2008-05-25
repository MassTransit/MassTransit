namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using Subscriptions;

	public class CorrelationIdDispatcher<T, V> :
		IMessageDispatcher
		where T : class, CorrelatedBy<V>
	{
		private readonly Dictionary<V, IMessageDispatcher> _dispatchers = new Dictionary<V, IMessageDispatcher>();
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;

		public CorrelationIdDispatcher(IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_builder = builder;
			_cache = cache;
			_bus = bus;
		}

		public CorrelationIdDispatcher()
		{
			
		}

		#region IMessageDispatcher Members

		public bool Accept(object obj)
		{
			return Accept((T) obj);
		}

		public void Consume(object obj)
		{
			Consume((T) obj);
		}

		public void Subscribe<TComponent>(TComponent component) where TComponent : class
		{
			Consumes<T>.For<V> consumer = component as Consumes<T>.For<V>;
			if (consumer == null)
				throw new ArgumentException(string.Format("The object does not support Consumes<{0}>.For<{1}>", typeof (T).Name, typeof (V).Name), "component");

			V correlationId = consumer.CorrelationId;

			IMessageDispatcher dispatcher = GetDispatcher(correlationId);

			dispatcher.Subscribe(consumer);
		}

		public void Unsubscribe<TComponent>(TComponent component) where TComponent : class
		{
			Consumes<T>.For<V> consumer = component as Consumes<T>.For<V>;
			if (consumer == null)
				throw new ArgumentException(string.Format("The object does not support Consumes<{0}>.For<{1}>", typeof (T).Name, typeof (V).Name), "component");

			V correlationId = consumer.CorrelationId;

			if (_dispatchers.ContainsKey(correlationId))
				_dispatchers[correlationId].Unsubscribe(consumer);
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			throw new NotImplementedException();
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			throw new NotImplementedException();
		}

		#endregion

		public void Dispose()
		{
			foreach (KeyValuePair<V, IMessageDispatcher> dispatcher in _dispatchers)
			{
				dispatcher.Value.Dispose();
			}

			_dispatchers.Clear();
		}

		private IMessageDispatcher GetDispatcher(V correlationId)
		{
			if (_dispatchers.ContainsKey(correlationId))
				return _dispatchers[correlationId];

			IMessageDispatcher dispatcher = new MessageDispatcher<T>(_bus, _cache, _builder);

			_dispatchers.Add(correlationId, dispatcher);

			return dispatcher;
		}

		public void Consume(T message)
		{
			CorrelatedBy<V> correlation = message;

			V correlationId = correlation.CorrelationId;

			if (_dispatchers.ContainsKey(correlationId))
			{
				_dispatchers[correlationId].Consume(message);
			}
		}

		public bool Accept(T message)
		{
			CorrelatedBy<V> correlation = message;

			V correlationId = correlation.CorrelationId;

			if (_dispatchers.ContainsKey(correlationId))
			{
				return _dispatchers[correlationId].Accept(message);
			}

			return false;
		}
	}
}