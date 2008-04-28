namespace MassTransit.ServiceBus
{
    public interface IProduce<T> where T : IMessage
    {
        void SetConsumer(IConsume<T> consumer);
    }
}