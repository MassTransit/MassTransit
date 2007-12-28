namespace MassTransit.ServiceBus
{
	public interface IMessageEndpointReceive
	{
		void OnMessageReceived(IServiceBus bus, IEnvelope envelope, IMessage message);
	}
}