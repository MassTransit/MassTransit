namespace MassTransit.Configuration
{
    using System.Net.Mime;


    public interface ISetSerializerMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        void SetSerializer(ContentType contentType);
    }
}
