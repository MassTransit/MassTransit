namespace MassTransit.MessageData.Converters
{
    using System.IO;
    using Metadata;


    public static class MessageDataConverter
    {
        public static readonly IMessageDataConverter<string> String = new StringMessageDataConverter();
        public static readonly IMessageDataConverter<byte[]> ByteArray = new ByteArrayMessageDataConverter();
        public static readonly IMessageDataConverter<Stream> Stream = new StreamMessageDataConverter();
    }
}
