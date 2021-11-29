namespace MassTransit.AzureServiceBusTransport
{
    using System;
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

        public Task Send(ServiceBusMessage message)
        {
            return _client.SendMessageAsync(message);
        }

        public Task<long> ScheduleSend(ServiceBusMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            return _client.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc);
        }

        public Task CancelScheduledSend(long sequenceNumber)
        {
            return _client.CancelScheduledMessageAsync(sequenceNumber);
        }
    }
}
