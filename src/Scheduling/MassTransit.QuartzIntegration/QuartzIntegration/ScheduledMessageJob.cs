namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Context;
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

            var contentType = new ContentType(jobData.GetString("ContentType")!);
            var destinationAddress = new Uri(jobData.GetString("Destination")!);
            var body = jobData.GetString("Body") ?? string.Empty;

            var supportedMessageTypes = (jobData.TryGetString("MessageType", out var text)
                ? text?.Split(';').ToArray()
                : default) ?? [];

            try
            {
                var pipe = new ForwardScheduledMessagePipe(contentType, messageContext, body, destinationAddress, supportedMessageTypes);

                var endpoint = await _bus.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                await endpoint.Send(new SerializedMessageBody(), pipe, context.CancellationToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {Key} {Schedule}", context.Trigger.Key, context.Trigger.GetNextFireTimeUtc());
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message: {MessageType} {DestinationAddress}", supportedMessageTypes, destinationAddress);

                throw new JobExecutionException(ex, context.RefireCount < 5);
            }
        }


        class ForwardScheduledMessagePipe :
            IPipe<SendContext>
        {
            readonly string _body;
            readonly ContentType? _contentType;
            readonly Uri? _destinationAddress;
            readonly string[] _supportedMessageTypes;
            readonly JobDataMessageContext _messageContext;

            public ForwardScheduledMessagePipe(ContentType? contentType, JobDataMessageContext messageContext, string body, Uri? destinationAddress,
                string[] supportedMessageTypes)
            {
                _contentType = contentType;
                _messageContext = messageContext;
                _body = body;
                _destinationAddress = destinationAddress;
                _supportedMessageTypes = supportedMessageTypes;
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

                if (_supportedMessageTypes.Any())
                    context.SupportedMessageTypes = _supportedMessageTypes;

                if (_messageContext.ExpirationTime.HasValue)
                    context.TimeToLive = _messageContext.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

                foreach (KeyValuePair<string, object> header in _messageContext.Headers.GetAll())
                    context.Headers.Set(header.Key, header.Value);

                IReadOnlyDictionary<string, object>? transportProperties = _messageContext.TransportProperties;
                if (transportProperties != null && context is TransportSendContext transportSendContext)
                    transportSendContext.ReadPropertiesFrom(transportProperties);

                context.Serializer = serializerContext.GetMessageSerializer();

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
