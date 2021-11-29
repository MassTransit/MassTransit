namespace MassTransit.MessageData.Conventions
{
    using MassTransit.Configuration;
    using Topology;


    public interface IMessageDataMessageConsumeTopologyConvention<TMessage> :
        IMessageConsumeTopologyConvention<TMessage>
        where TMessage : class
    {
    }
}
