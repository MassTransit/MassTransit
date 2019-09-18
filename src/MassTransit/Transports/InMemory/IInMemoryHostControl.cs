namespace MassTransit.Transports.InMemory
{
    using Builders;
    using Context;
    using Topology.Builders;


    public interface IInMemoryHostControl :
        IInMemoryHost,
        IBusHostControl,
        ISendTransportProvider
    {
        IReceiveTransport GetReceiveTransport(string queueName, ReceiveEndpointContext receiveEndpointContext);

        IInMemoryPublishTopologyBuilder CreatePublishTopologyBuilder(
            PublishEndpointTopologyBuilder.Options options = PublishEndpointTopologyBuilder.Options.MaintainHierarchy);

        IInMemoryConsumeTopologyBuilder CreateConsumeTopologyBuilder();
    }
}
