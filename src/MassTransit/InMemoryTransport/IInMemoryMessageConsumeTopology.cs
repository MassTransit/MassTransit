namespace MassTransit
{
    public interface IInMemoryMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
