namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;


    public class MessageSendEndpointContext :
        BasePipeContext,
        SendEndpointContext
    {
        readonly IMessageSender _client;

        public MessageSendEndpointContext(ConnectionContext connectionContext, IMessageSender client)
        {
            _client = client;
            ConnectionContext = connectionContext;
        }

        public ConnectionContext ConnectionContext { get; }

        public string EntityPath => _client.Path;

        public Task Send(Message message)
        {
            return _client.SendAsync(message);
        }

        public Task<long> ScheduleSend(Message message, DateTime scheduleEnqueueTimeUtc)
        {
            return _client.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc);
        }

        public Task CancelScheduledSend(long sequenceNumber)
        {
            return _client.CancelScheduledMessageAsync(sequenceNumber);
        }
    }
}
