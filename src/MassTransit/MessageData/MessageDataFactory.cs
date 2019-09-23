namespace MassTransit.MessageData
{
    using System;
    using System.Threading;
    using Metadata;


    public static class MessageDataFactory
    {
        public static MessageData<T> Load<T>(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
        {
            if (typeof(T) == typeof(string))
                return (MessageData<T>)new LoadMessageData<string>(address, repository, MessageDataConverter.String, cancellationToken);

            if (typeof(T) == typeof(byte[]))
                return (MessageData<T>)new LoadMessageData<byte[]>(address, repository, MessageDataConverter.ByteArray, cancellationToken);

            throw new MessageDataException("Unknown message data type: " + TypeMetadataCache<T>.ShortName);
        }
    }
}
