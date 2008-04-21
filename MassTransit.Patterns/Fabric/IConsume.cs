namespace MassTransit.Patterns.Fabric
{
	using System;
	using ServiceBus;

	public interface IConsume<TMessage> :
		IDisposable
		where TMessage : IMessage
	{
		void Consume(TMessage message);
	}
}