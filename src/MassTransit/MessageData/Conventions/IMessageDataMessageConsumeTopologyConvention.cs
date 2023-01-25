namespace MassTransit.MessageData.Conventions
{
    using MassTransit.Configuration;


    public interface IMessageDataMessageConsumeTopologyConvention<TMessage> :
        IMessageConsumeTopologyConvention<TMessage>
        where TMessage : class
    {
    }
}
