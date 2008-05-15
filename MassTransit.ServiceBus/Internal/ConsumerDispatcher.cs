namespace MassTransit.ServiceBus.Internal
{
	using System.Collections.Generic;

	public class ConsumerDispatcher<T> :
		IConsumerDispatcher<T> where T : class
	{
		private readonly List<Consumes<T>.Any> _consumers = new List<Consumes<T>.Any>();

		#region IConsumerDispatcher<T> Members

		public bool Dispatch(T message)
		{
			bool result = false;

			foreach (Consumes<T>.Any consumer in _consumers)
			{
				result = true;
				consumer.Consume(message);
			}

			return result;
		}

		public void Subscribe(Consumes<T>.Any consumer)
		{
			if (!_consumers.Contains(consumer))
				_consumers.Add(consumer);
		}

		public void Unsubscribe(Consumes<T>.Any consumer)
		{
			if (_consumers.Contains(consumer))
				_consumers.Remove(consumer);
		}

		#endregion
	}
}