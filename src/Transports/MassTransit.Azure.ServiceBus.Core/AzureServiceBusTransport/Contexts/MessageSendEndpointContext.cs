namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using MassTransit.Middleware;


    public class MessageSendEndpointContext :
        BasePipeContext,
        SendEndpointContext
    {
        readonly ServiceBusSender _client;

        public MessageSendEndpointContext(ConnectionContext connectionContext, ServiceBusSender client)
        {
            _client = client;
            ConnectionContext = connectionContext;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _client.EntityPath;

        public Task Send(ServiceBusMessage message, CancellationToken cancellationToken)
        {
            return _client.SendMessageAsync(message, cancellationToken);
        }

        public Task<long> ScheduleSend(ServiceBusMessage message, DateTime scheduleEnqueueTimeUtc, CancellationToken cancellationToken)
        {
            return _client.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc, cancellationToken);
        }

        public Task CancelScheduledSend(long sequenceNumber, CancellationToken cancellationToken)
        {
            return _client.CancelScheduledMessageAsync(sequenceNumber, cancellationToken);
        }
    }
}
