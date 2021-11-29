namespace MassTransit.MessageData.PropertyProviders
{
    using System.IO;
    using Metadata;


    public static class MessageDataReaderFactory
    {
        public static IMessageDataReader<T> CreateReader<T>()
        {
            if (typeof(T) == typeof(string))
                return new StringMessageDataReader<T>();

            if (typeof(T) == typeof(byte[]))
                return new BytesMessageDataReader<T>();

            if (typeof(T) == typeof(Stream))
                return new StreamMessageDataReader<T>();

            if (TypeMetadataCache.IsValidMessageDataType(typeof(T)))
                return new ObjectMessageDataReader<T>();

            throw new MessageDataException("Unsupported message data type: " + TypeCache<T>.ShortName);
        }
    }
}
