namespace MassTransit.GrpcTransport.Topology
{
    using MassTransit.Topology;


    public interface IGrpcMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
