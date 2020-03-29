namespace MassTransit.MessageData.Conventions
{
    using Topology;


    public interface IMessageDataMessageConsumeTopologyConvention<TMessage> :
        IMessageConsumeTopologyConvention<TMessage>
        where TMessage : class
    {
    }
}
