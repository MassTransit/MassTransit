namespace MassTransit.Patterns.Fabric
{
	using System.Collections.Generic;
	using ServiceBus;

	public class MessageRouter<TMessage> : 
		IConsume<TMessage>,
		IProduce<TMessage>
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
				AttachConsumer(consumer);
			}
		}

		public void Consume(TMessage message)
		{
			foreach (IConsume<TMessage> consumer in _consumers)
			{
				consumer.Consume(message);
			}
		}

		public void AttachConsumer(IConsume<TMessage> consumer)
		{
			if (_consumers.Contains(consumer))
				return;

			_consumers.Add(consumer);
		}

		public void DetachConsumer(IConsume<TMessage> consumer)
		{
			if (_consumers.Contains(consumer))
				_consumers.Remove(consumer);
		}

		public void Dispose()
		{
			_consumers.Clear();
		}
	}
}