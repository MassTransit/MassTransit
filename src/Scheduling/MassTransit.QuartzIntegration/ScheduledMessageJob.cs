namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Context;

    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;

    using Newtonsoft.Json;

    using Quartz;

    using Serialization;


    public class ScheduledMessageJob :
        IJob
    {
        internal const string BUS_CONTEXT_KEY = "MassTransit.Bus";

        readonly IBus _bus;

        public ScheduledMessageJob()
        {

        }

        public ScheduledMessageJob(IBus bus)
        {
            _bus = bus;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var messageData = new ScheduledMessageData();
            var jobData = new JobDataMap();
            jobData.PutAll(context.Scheduler.Context);
            jobData.PutAll(context.JobDetail.JobDataMap);
            jobData.PutAll(context.Trigger.JobDataMap);
            jobData.Put("PayloadMessageHeadersAsJson", CreatePayloadHeaderString(context));
            SetObjectProperties(messageData, jobData);

            try
            {
                var bus = (IBus)context.Scheduler.Context[BUS_CONTEXT_KEY] ?? _bus;
                if (bus == null)
                    throw new Exception("Could not find MassTransit Bus instance on the Job or the Scheduler Context.");

                var destinationAddress = messageData.Destination;
                var sourceAddress = bus.Address;

                IPipe<SendContext> sendPipe = CreateMessageContext(messageData, sourceAddress);

                var endpoint = await bus.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, context.CancellationToken).ConfigureAwait(false);

                LogContext.Debug?.Log("Schedule Executed: {Key} {Schedule}", context.JobDetail.Key, context.Trigger.GetNextFireTimeUtc());
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message, type: {MessageType}, destination: {DestinationAddress}", messageData.MessageType, messageData.Destination);

                throw new JobExecutionException(ex, context.RefireCount < 5);
            }
        }

        IPipe<SendContext> CreateMessageContext(ScheduledMessageData messageData, Uri sourceAddress)
        {
            return new SerializedMessageContextAdapter(Pipe.Empty<SendContext>(), messageData, sourceAddress);
        }


        static void SetObjectProperties(ScheduledMessageData job, JobDataMap jobData)
        {
            foreach (var key in jobData.Keys)
            {
                if (TypeCache<ScheduledMessageData>.ReadWritePropertyCache.TryGetProperty(key, out ReadWriteProperty<ScheduledMessageData> property))
                {
                    var value = jobData[key];

                    if (property.Property.PropertyType == typeof(Uri))
                        value = new Uri(value.ToString());

                    property.Set(job, value);
                }
            }
        }

        /// <summary>
        /// Some additional properties from the TriggerFiredBundle
        /// There is a bug in RabbitMq.Client that prevents using the DateTimeOffset type in the headers
        /// These values are being serialized as ISO-8601 round trip string
        /// </summary>
        /// <param name="bundle"></param>
        static string CreatePayloadHeaderString(IJobExecutionContext bundle)
        {
            var timeHeaders = new Dictionary<string, object> { { MessageHeaders.Quartz.Sent, bundle.FireTimeUtc } };
            if (bundle.ScheduledFireTimeUtc.HasValue)
                timeHeaders.Add(MessageHeaders.Quartz.Scheduled, bundle.ScheduledFireTimeUtc);

            if (bundle.NextFireTimeUtc.HasValue)
                timeHeaders.Add(MessageHeaders.Quartz.NextScheduled, bundle.NextFireTimeUtc);

            if (bundle.PreviousFireTimeUtc.HasValue)
                timeHeaders.Add(MessageHeaders.Quartz.PreviousSent, bundle.PreviousFireTimeUtc);

            if (bundle.JobDetail.JobDataMap.TryGetValue("TokenId", out var tokenId))
                timeHeaders.Add(MessageHeaders.SchedulingTokenId, tokenId);

            return JsonConvert.SerializeObject(timeHeaders);
        }

        /// <summary>
        /// Used to store job data inside the scheduler.
        /// </summary>
        class ScheduledMessageData : SerializedMessage
        {

            public Uri Destination { get; set; }

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

        }

        class Scheduled
        {
        }
    }
}
