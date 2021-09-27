namespace MassTransit.MessageData.PropertyProviders
{
    using System;
    using System.Threading;
    using Values;


    public class ObjectMessageDataReader<T> :
        IMessageDataReader<T>
    {
        readonly IMessageDataConverter<T> _converter;

        public ObjectMessageDataReader()
        {
            _converter = new ObjectMessageDataConverter<T>();
        }

        public MessageData<T> GetMessageData(IMessageDataRepository repository, Uri address, CancellationToken cancellationToken)
        {
            return new GetMessageData<T>(address, repository, _converter, cancellationToken);
        }
    }
}
