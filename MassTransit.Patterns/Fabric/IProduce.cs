namespace MassTransit.Patterns.Fabric
{
	using ServiceBus;

	public interface IProduce<TMessage> where TMessage : IMessage
	{
		void Attach(IConsume<TMessage> consumer);
	}
}