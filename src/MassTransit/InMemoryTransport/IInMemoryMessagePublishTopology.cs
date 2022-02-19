namespace MassTransit
{
    using Configuration;
    using Transports.Fabric;


    public interface IInMemoryMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IInMemoryMessagePublishTopology
        where TMessage : class
    {
        ExchangeType ExchangeType { get; }
    }


    public interface IInMemoryMessagePublishTopology
    {
        void Apply(IMessageFabricPublishTopologyBuilder builder);
    }
}
