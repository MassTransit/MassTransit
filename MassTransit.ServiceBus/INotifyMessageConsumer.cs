namespace MassTransit.ServiceBus
{
	public interface INotifyMessageConsumer
	{
		void Deliver(IServiceBus bus, IEnvelope envelope, IMessage message);
	    bool MeetsCriteria(IMessage message);
	}
}