namespace MassTransit.Patterns.Fabric
{
	using System;
	using ServiceBus;

	public interface IProduce<TMessage> :
		IDisposable
		where TMessage : IMessage
	{
		void Attach(IConsume<TMessage> consumer);
		void Detach(IConsume<TMessage> consumer);
	}
}