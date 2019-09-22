namespace MassTransit.Metadata
{
    using Contracts;


    public interface IMessageInfoCache
    {
        MessageInfo GetMessageInfo<T>()
            where T : class;
    }
}
