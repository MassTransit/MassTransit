namespace MassTransit.ServiceBus
{
	public interface INotifyMessageConsumer
	{
		void OnMessageReceived(IServiceBus bus, IEnvelope envelope, IMessage message);
	}
}