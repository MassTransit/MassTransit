namespace MassTransit.Patterns.Fabric
{
	using System;
	using System.Collections.Generic;

	public class MessageRouter<TMessage> :
		Dispatches<TMessage>,
		IDisposable
	{
		private readonly List<Consumes<TMessage>> _consumers = new List<Consumes<TMessage>>();

		public MessageRouter()
		{
		}

		public MessageRouter(params Consumes<TMessage>[] consumers)
		{
			foreach (Consumes<TMessage> consumer in consumers)
			{
				Attach(consumer);
			}
		}

		#region Dispatches<TMessage> Members

		public void Consume(TMessage message)
		{
			foreach (Consumes<TMessage> consumer in _consumers)
			{
				consumer.Consume(message);
			}
		}

		public void Attach(Consumes<TMessage> consumer)
		{
			if (_consumers.Contains(consumer))
				return;

			_consumers.Add(consumer);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_consumers.Clear();
		}

		#endregion
	}
}