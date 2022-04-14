namespace MassTransit.Context
{
    using System.Collections.Generic;


    public interface TransportSendContext :
        PublishContext
    {
        void WritePropertiesTo(IDictionary<string, object> properties);

        void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties);
    }


    public interface TransportSendContext<out TMessage> :
        PublishContext<TMessage>,
        TransportSendContext
        where TMessage : class
    {
    }
}
