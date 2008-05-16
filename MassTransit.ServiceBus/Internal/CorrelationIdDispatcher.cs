namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;

	public class CorrelationIdDispatcher<T, V> :
		IMessageDispatcher
		where T : class, CorrelatedBy<V>
	{
		private readonly Dictionary<V, IMessageDispatcher> _dispatchers = new Dictionary<V, IMessageDispatcher>();

		#region IMessageDispatcher Members

		public bool Dispatch(object obj)
		{
			return Dispatch((T) obj);
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
				throw new ArgumentException(string.Format("The object does not support Consumes<{0}>.For<{1}>", typeof(T).Name, typeof(V).Name), "component");

			V correlationId = consumer.CorrelationId;

			if (_dispatchers.ContainsKey(correlationId))
				_dispatchers[correlationId].Unsubscribe(consumer);
		}

		public void AddComponent<TComponent>()
		{
			throw new NotImplementedException();
		}

		#endregion

		private IMessageDispatcher GetDispatcher(V correlationId)
		{
			if (_dispatchers.ContainsKey(correlationId))
				return _dispatchers[correlationId];

			IMessageDispatcher dispatcher = new MessageDispatcher<T>();

			_dispatchers.Add(correlationId, dispatcher);

			return dispatcher;
		}

		public bool Dispatch(T message)
		{
			CorrelatedBy<V> correlation = message;

			V correlationId = correlation.CorrelationId;

			if (_dispatchers.ContainsKey(correlationId))
			{
				return _dispatchers[correlationId].Dispatch(message);
			}

			return false;
		}
	}
}