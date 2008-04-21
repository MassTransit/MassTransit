namespace MassTransit.Patterns.Fabric
{
	using System;
	using ServiceBus;

	public interface IProduce<TMessage> :
		IDisposable
		where TMessage : IMessage
	{
		void AttachConsumer(IConsume<TMessage> consumer);
		void DetachConsumer(IConsume<TMessage> consumer);
	}
}