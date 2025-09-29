namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Send(ServiceBusMessage message, CancellationToken cancellationToken);

        /// <summary>
        /// Schedule a send in the future to the messaging entity
        /// </summary>
        /// <param name="message"></param>
        /// <param name="scheduleEnqueueTimeUtc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> ScheduleSend(ServiceBusMessage message, DateTime scheduleEnqueueTimeUtc, CancellationToken cancellationToken);

        /// <summary>
        /// Cancel a previously schedule send on the messaging entity
        /// </summary>
        /// <param name="sequenceNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CancelScheduledSend(long sequenceNumber, CancellationToken cancellationToken);
    }
}
