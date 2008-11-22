namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections.Generic;

	public class MessagePipeline<TMessage> :
		IMessageSink<TMessage>
		where TMessage : class
	{
		private readonly IMessageSink<TMessage> _outputSink;

		public MessagePipeline(IMessageSink<TMessage> outputSink)
		{
			_outputSink = outputSink;
		}

		public void Dispatch(TMessage message, Func<TMessage, bool> accept)
		{
			foreach (Consumes<TMessage>.All consumer in Enumerate(message))
			{
				if (!accept(message))
					break;

				accept = x => true;

				consumer.Consume(message);
			}
		}

		public IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			foreach (Consumes<TMessage>.All consumer in _outputSink.Enumerate(message))
			{
				yield return consumer;
			}
		}
	}
}