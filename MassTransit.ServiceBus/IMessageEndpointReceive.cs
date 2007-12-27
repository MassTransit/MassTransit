namespace MassTransit.ServiceBus
{
	public interface IMessageEndpointReceive
	{
		void OnMessageReceived(IEnvelope envelope, IMessage message);
	}
}