namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections.Generic;

	public class MessageRouter<TMessage> :
		IMessageSink<TMessage>
		where TMessage : class
	{
		public IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			foreach (IMessageSink<TMessage> sink in _sinks)
			{
				foreach (Consumes<TMessage>.All consumer in sink.Enumerate(message))
				{
					yield return consumer;
				}
			}
		}

		private readonly List<IMessageSink<TMessage>> _sinks = new List<IMessageSink<TMessage>>();

		public Func<bool> Add(IMessageSink<TMessage> sink)
		{
			_sinks.Add(sink);

			return () => _sinks.Remove(sink);
		}
	}
}