namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Quartz;
    using Serialization;


    public class ScheduledMessageJob :
        IJob,
        SerializedMessage
    {
        readonly IBus _bus;

        public ScheduledMessageJob(IBus bus)
        {
            _bus = bus;
        }

        public string Destination { get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var destinationAddress = new Uri(Destination);
                var sourceAddress = _bus.Address;

                IPipe<SendContext> sendPipe = CreateMessageContext(sourceAddress, context.Trigger.Key.Name);

                var endpoint = await _bus.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, context.CancellationToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {Key} {Schedule}", context.JobDetail.Key, context.Trigger.GetNextFireTimeUtc());
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message, type: {MessageType}, destination: {DestinationAddress}", MessageType, Destination);

                throw new JobExecutionException(ex, context.RefireCount < 5);
            }
        }

        public string ExpirationTime { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string Body { get; set; }
        public string MessageId { get; set; }
        public string MessageType { get; set; }
        public string ContentType { get; set; }
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
        public string ConversationId { get; set; }
        public string InitiatorId { get; set; }
        public string TokenId { get; set; }
        public string HeadersAsJson { get; set; }
        public string PayloadMessageHeadersAsJson { get; set; }

        Uri SerializedMessage.Destination => new Uri(Destination);

        IPipe<SendContext> CreateMessageContext(Uri sourceAddress, string triggerKey)
        {
            IPipe<SendContext> sendPipe = Pipe.Execute<SendContext>(context =>
            {
                Guid? tokenId = ConvertIdToGuid(TokenId);
                if (tokenId.HasValue)
                    context.Headers.Set(MessageHeaders.SchedulingTokenId, tokenId.Value.ToString("N"));

                context.Headers.Set(MessageHeaders.QuartzTriggerKey, triggerKey);
            });

            return new SerializedMessageContextAdapter(sendPipe, this, sourceAddress);
        }

        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }


        class Scheduled
        {
        }
    }
}
