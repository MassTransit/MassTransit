namespace MassTransit.MessageData.PropertyProviders
{
    using System;
    using System.Threading;
    using Converters;
    using Values;


    public class StringMessageDataReader<T> :
        IMessageDataReader<T>
    {
        public MessageData<T> GetMessageData(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
        {
            return (MessageData<T>)new GetMessageData<string>(address, repository, MessageDataConverter.String, cancellationToken);
        }
    }
}
