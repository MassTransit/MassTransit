namespace MassTransit.Patterns.Fabric
{
	using ServiceBus;

	public interface IDispatcher<TMessage> :
		IConsume<TMessage>,
		IProduce<TMessage>
		where TMessage : IMessage
	{
	}
}