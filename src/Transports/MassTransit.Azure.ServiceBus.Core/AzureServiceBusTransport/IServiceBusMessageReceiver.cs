namespace MassTransit.AzureServiceBusTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;


    public interface IServiceBusMessageReceiver
    {
        /// <summary>
        /// Handles the <paramref name="message" />
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">Specify an optional cancellationToken</param>
        /// <returns></returns>
        Task Handle(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default);
    }
}
