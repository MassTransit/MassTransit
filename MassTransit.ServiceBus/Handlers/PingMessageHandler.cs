namespace MassTransit.ServiceBus
{
    public class PingMessageHandler :
        BaseMessageHandler<PingMessage>
    {
        public PingMessageHandler(IServiceBus bus) : 
            base(bus)
        {
        }

        public override void Handle(PingMessage message)
        {
            
        }
    }
}