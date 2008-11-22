namespace MassTransit.ServiceBus.Tests
{
	using System.Collections.Generic;

	public interface IMessageSink<TMessage> where TMessage : class
	{
		IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message);
	}
}