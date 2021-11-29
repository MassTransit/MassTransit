namespace MassTransit
{
    public interface IAmazonSqsMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
