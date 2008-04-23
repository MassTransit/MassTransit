namespace MassTransit.Patterns.Fabric
{
	using ServiceBus;

	public interface IConsume<TMessage> where TMessage : IMessage
	{
		void Consume(TMessage message);
	}
}