namespace MassTransit.ServiceBus
{
    public interface IMessageQueueEndpoint :
        IEndpoint
    {
        /// <summary>
        /// Returns a MessageQueue name for use with MessageQueue.Open
        /// </summary>
        string QueueName { get; }
    }
}