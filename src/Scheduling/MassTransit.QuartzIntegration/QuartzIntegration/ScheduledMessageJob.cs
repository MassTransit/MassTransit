#nullable enable
namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Quartz;
    using Serialization;


    public class ScheduledMessageJob :
        IJob
    {
        readonly IBus _bus;

        public ScheduledMessageJob(IBus bus)
        {
            _bus = bus;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobData = context.MergedJobDataMap;
            var messageContext = new JobDataMessageContext(context, SystemTextJsonMessageSerializer.Instance);

            var contentType = new ContentType(jobData.GetString("ContentType"));
            var destinationAddress = new Uri(jobData.GetString("Destination"));
            var body = jobData.GetString("Body") ?? string.Empty;
            var messageType = jobData.GetString("MessageType")?.Split(';')?.ToArray() ?? Array.Empty<string>();

            try
            {
                var pipe = new ForwardScheduledMessagePipe(contentType, messageContext, body, destinationAddress);

                var endpoint = await _bus.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, pipe, context.CancellationToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {Key} {Schedule}", context.JobDetail.Key, context.Trigger.GetNextFireTimeUtc());
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {MessageType} {DestinationAddress}", messageType, destinationAddress);

                throw new JobExecutionException(ex, context.RefireCount < 5);
            }
        }


        class Scheduled
        {
        }


        class ForwardScheduledMessagePipe :
            IPipe<SendContext>
        {
            readonly string _body;
            readonly ContentType? _contentType;
            readonly Uri? _destinationAddress;
            readonly JobDataMessageContext _messageContext;

            public ForwardScheduledMessagePipe(ContentType? contentType, JobDataMessageContext messageContext, string body, Uri? destinationAddress)
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

                if (_messageContext.MessageId.HasValue)
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
    }
}
