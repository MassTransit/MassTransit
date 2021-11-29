namespace MassTransit
{
    public interface IRedeliveryConfigurator :
        IRetryConfigurator
    {
        /// <summary>
        /// Generate a new MessageId for each redelivered message, replacing the original
        /// MessageId. This is commonly done when using transport-level de-duplication
        /// with Azure Service Bus or Amazon SQS.
        /// </summary>
        bool ReplaceMessageId { set; }
    }
}
