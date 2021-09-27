namespace MassTransit.MessageData.PropertyProviders
{
    using System;
    using System.Threading;


    public interface IMessageDataReader<T>
    {
        MessageData<T> GetMessageData(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken);
    }
}
