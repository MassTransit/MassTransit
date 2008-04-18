namespace MassTransit.ServiceBus
{
    public interface IConsume<T> where T : IMessage
    {
        void Handle(IMessageContext<T> ctx);
    }
}
