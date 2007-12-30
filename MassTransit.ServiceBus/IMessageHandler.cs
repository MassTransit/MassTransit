namespace MassTransit.ServiceBus
{
    public interface IMessageHandler<T> where T : IMessage
    {
        void Handle(MessageContext<T> context);
    }
}