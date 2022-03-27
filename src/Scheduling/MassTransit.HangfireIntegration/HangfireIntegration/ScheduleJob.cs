namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Server;
    using Serialization;


    [Queue(HangfireEndpointOptions.DefaultQueueName)]
    class ScheduleJob
    {
        readonly IBus _bus;

        public ScheduleJob(IBus bus)
        {
            _bus = bus;
        }

        [HashCleanup]
        public async Task SendMessage(HangfireScheduledMessageData messageData, PerformContext performContext)
        {
            try
            {
                var messageContext = new MessageDataMessageContext(messageData, SystemTextJsonMessageSerializer.Instance);
                var contentType = string.IsNullOrWhiteSpace(messageData.ContentType) ? default : new ContentType(messageData.ContentType);
                var body = messageData.Body ?? string.Empty;

                var pipe = new ForwardScheduledMessagePipe(contentType, messageContext, body, messageData.Destination);

                var endpoint = await _bus.GetSendEndpoint(messageData.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, pipe, performContext.CancellationToken.ShutdownToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {JobId}, created at: {CreatedAt}", performContext.BackgroundJob.Id,
                    performContext.BackgroundJob.CreatedAt);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {JobId}, created at: {CreatedAt}, destination: {DestinationAddress}",
                    performContext.BackgroundJob.Id, messageData.Destination, performContext.BackgroundJob.CreatedAt);

                throw new JobPerformanceException("Job Execution exception", ex);
            }
        }

        [RecurringScheduleDateTimeInterval]
        public async Task SendMessage(HangfireRecurringScheduledMessageData messageData, PerformContext performContext)
        {
            try
            {
                var messageContext = new MessageDataMessageContext(messageData, SystemTextJsonMessageSerializer.Instance);
                var contentType = string.IsNullOrWhiteSpace(messageData.ContentType) ? default : new ContentType(messageData.ContentType);
                var body = messageData.Body ?? string.Empty;

                var pipe = new ForwardScheduledMessagePipe(contentType, messageContext, body, messageData.Destination);

                var endpoint = await _bus.GetSendEndpoint(messageData.Destination).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, pipe, performContext.CancellationToken.ShutdownToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {JobId}, created at: {CreatedAt}, with range: {StartTime}-{EndTime}", performContext.BackgroundJob.Id,
                    performContext.BackgroundJob.CreatedAt, messageData.StartTime, messageData.EndTime);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {JobId}, created at: {CreatedAt}, destination: {DestinationAddress}",
                    performContext.BackgroundJob.Id, messageData.Destination, performContext.BackgroundJob.CreatedAt);

                throw new JobPerformanceException("Job Execution exception", ex);
            }
        }


        class ForwardScheduledMessagePipe :
            IPipe<SendContext>
        {
            readonly string _body;
            readonly ContentType? _contentType;
            readonly Uri? _destinationAddress;
            readonly MessageDataMessageContext _messageContext;

            public ForwardScheduledMessagePipe(ContentType? contentType, MessageDataMessageContext messageContext, string body, Uri? destinationAddress)
            {
                _contentType = contentType;
                _messageContext = messageContext;
                _body = body;
                _destinationAddress = destinationAddress;
            }

            public Task Send(SendContext context)
            {
                var deserializer = context.Serialization.GetMessageDeserializer(_contentType);

                var body = deserializer.GetMessageBody(_body);

                var serializerContext = deserializer.Deserialize(body, _messageContext, _destinationAddress);

                context.MessageId = _messageContext.MessageId;
                context.RequestId = _messageContext.RequestId;
                context.ConversationId = _messageContext.ConversationId;
                context.CorrelationId = _messageContext.CorrelationId;
                context.InitiatorId = _messageContext.InitiatorId;
                context.SourceAddress = _messageContext.SourceAddress;
                context.ResponseAddress = _messageContext.ResponseAddress;
                context.FaultAddress = _messageContext.FaultAddress;

                if (_messageContext.ExpirationTime.HasValue)
                    context.TimeToLive = _messageContext.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

                foreach (KeyValuePair<string, object> header in _messageContext.Headers.GetAll())
                    context.Headers.Set(header.Key, header.Value);

                context.Serializer = serializerContext.GetMessageSerializer();

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class Scheduled
        {
        }
    }
}
