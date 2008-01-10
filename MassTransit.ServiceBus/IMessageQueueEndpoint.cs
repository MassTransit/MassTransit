namespace MassTransit.ServiceBus
{
	/// <summary>
	/// An extension of the IEndpoint interface for the additional support of Message Queue backed endpoints
	/// </summary>
    public interface IMessageQueueEndpoint :
        IEndpoint
    {
        /// <summary>
        /// The path of the message queue for the endpoint. Suitable for use with <c ref="MessageQueue" />.Open
        /// to access a message queue.
        /// </summary>
        string QueueName { get; }
    }
}