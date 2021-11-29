namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;


    public interface SendEndpointContext :
        NamespaceContext
    {
        /// <summary>
        /// The path of the messaging entity
        /// </summary>
        string EntityPath { get; }

        /// <summary>
        /// Send the message to the messaging entity
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Send(ServiceBusMessage message);

        /// <summary>
        /// Schedule a send in the future to the messaging entity
        /// </summary>
        /// <param name="message"></param>
        /// <param name="scheduleEnqueueTimeUtc"></param>
        /// <returns></returns>
        Task<long> ScheduleSend(ServiceBusMessage message, DateTime scheduleEnqueueTimeUtc);

        /// <summary>
        /// Cancel a previously schedule send on the messaging entity
        /// </summary>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        Task CancelScheduledSend(long sequenceNumber);
    }
}
