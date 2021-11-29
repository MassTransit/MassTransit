namespace MassTransit
{
    /// <summary>
    /// Used to configure the binding of an exchange (to either a queue or another exchange)
    /// </summary>
    public interface IAmazonSqsTopicSubscriptionConfigurator :
        IAmazonSqsTopicConfigurator
    {
    }
}
