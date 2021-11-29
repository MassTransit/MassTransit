namespace MassTransit.Configuration
{
    /// <summary>
    /// A convention that is applies to a message type on Publish, if applicable to
    /// the message type.
    /// </summary>
    public interface IPublishTopologyConvention :
        IMessagePublishTopologyConvention
    {
    }
}
