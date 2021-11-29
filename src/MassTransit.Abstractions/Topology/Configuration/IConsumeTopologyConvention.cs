namespace MassTransit.Configuration
{
    /// <summary>
    /// A convention that is applies to a message type on Consume, if applicable to
    /// the message type.
    /// </summary>
    public interface IConsumeTopologyConvention :
        IMessageConsumeTopologyConvention
    {
    }
}
