namespace MassTransit
{
    public interface IGroupKeyProvider<in TMessage, TKey>
        where TMessage : class
    {
        bool TryGetKey(ConsumeContext<TMessage> context, out TKey key);
    }
}
