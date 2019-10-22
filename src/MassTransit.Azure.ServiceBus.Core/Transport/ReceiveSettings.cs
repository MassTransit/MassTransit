namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using Microsoft.Azure.ServiceBus.Management;


    public interface ReceiveSettings :
        ClientSettings
    {
        QueueDescription GetQueueDescription();

        /// <summary>
        /// If TRUE, subscriptions will be removed on shutdown to avoid overflowing the topic
        /// </summary>
        bool RemoveSubscriptions { get; }
    }
}
