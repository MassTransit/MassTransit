namespace MassTransit.ServiceBus.Internal
{
	using System.Collections.Generic;

	public class CorrelationIdDispatcher<T, V> :
		IExternalMessageDispatcher
		where T : class, CorrelatedBy<V>
	{
		private readonly Dictionary<V, IConsumerDispatcher<T>> _dispatchers = new Dictionary<V, IConsumerDispatcher<T>>();

		public bool Dispatch(object obj)
		{
			return Dispatch((T) obj);
		}

		public void Subscribe<TComponent>(TComponent component) where TComponent : class
		{
			Consumes<T>.For<V> consumer = component as Consumes<T>.For<V>;
			if (consumer != null)
			{
				V correlationId = consumer.CorrelationId;

				IConsumerDispatcher<T> dispatcher = new ConsumerDispatcher<T>();

				_dispatchers.Add(correlationId, dispatcher);

				dispatcher.Subscribe(consumer);
			}
		}

		public bool Dispatch(T message)
		{
			CorrelatedBy<V> correlation = message;

			V key = correlation.CorrelationId;

			if (_dispatchers.ContainsKey(key))
			{
				return _dispatchers[key].Dispatch(message);
			}

			return false;
		}
	}
}