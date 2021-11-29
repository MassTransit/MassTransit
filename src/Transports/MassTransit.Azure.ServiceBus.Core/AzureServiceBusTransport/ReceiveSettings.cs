namespace MassTransit.AzureServiceBusTransport
{
    using Azure.Messaging.ServiceBus.Administration;


    public interface ReceiveSettings :
        ClientSettings
    {
        /// <summary>
        /// If TRUE, subscriptions will be removed on shutdown to avoid overflowing the topic
        /// </summary>
        bool RemoveSubscriptions { get; }

        CreateQueueOptions GetCreateQueueOptions();
    }
}
