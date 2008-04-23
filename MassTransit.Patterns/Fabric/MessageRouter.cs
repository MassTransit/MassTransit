namespace MassTransit.Patterns.Fabric
{
	using System;
	using System.Collections.Generic;
	using ServiceBus;

	public class MessageRouter<TMessage> :
		IDispatcher<TMessage>,
		IDisposable
		where TMessage : IMessage
	{
		private readonly List<IConsume<TMessage>> _consumers = new List<IConsume<TMessage>>();

		public MessageRouter()
		{
		}

		public MessageRouter(params IConsume<TMessage>[] consumers)
		{
			foreach (IConsume<TMessage> consumer in consumers)
			{
				Attach(consumer);
			}
		}

		#region IDispatcher<TMessage> Members

		public void Consume(TMessage message)
		{
			foreach (IConsume<TMessage> consumer in _consumers)
			{
				consumer.Consume(message);
			}
		}

		public void Attach(IConsume<TMessage> consumer)
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