namespace MassTransit.MessageData.PropertyProviders
{
    using System;
    using System.Threading;
    using Metadata;
    using Values;


    public static class MessageDataFactory
    {
        public static MessageData<T> Load<T>(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
        {
            if (typeof(T) == typeof(string))
                return (MessageData<T>)new GetMessageData<string>(address, repository, MessageDataConverter.String, cancellationToken);

            if (typeof(T) == typeof(byte[]))
                return (MessageData<T>)new GetMessageData<byte[]>(address, repository, MessageDataConverter.ByteArray, cancellationToken);

            throw new MessageDataException("Unsupported message data type: " + TypeMetadataCache<T>.ShortName);
        }
    }
}
