namespace MassTransit.MessageData
{
    using Serialization.JsonConverters;


    public interface IInlineMessageData
    {
        void Set(MessageDataReference reference);
    }
}
