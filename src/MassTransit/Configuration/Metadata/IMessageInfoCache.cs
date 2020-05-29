namespace MassTransit.Metadata
{
    using Contracts.Metadata;


    public interface IMessageInfoCache
    {
        MessageInfo GetMessageInfo<T>()
            where T : class;
    }
}
