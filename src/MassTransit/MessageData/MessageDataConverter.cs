namespace MassTransit.MessageData
{
    public static class MessageDataConverter
    {
        public static readonly IMessageDataConverter<string> String = new StringMessageDataConverter();
        public static readonly IMessageDataConverter<byte[]> ByteArray = new ByteArrayMessageDataConverter();
    }
}
