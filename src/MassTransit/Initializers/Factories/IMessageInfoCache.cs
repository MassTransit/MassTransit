namespace MassTransit.Initializers.Factories
{
    using Contracts;


    public interface IMessageInfoCache
    {
        MessageInfo GetMessageInfo<T>()
            where T : class;
    }
}
