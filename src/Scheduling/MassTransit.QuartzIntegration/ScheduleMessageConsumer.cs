namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Context;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Quartz;
    using Quartz.Util;
    using Scheduling;
    using Serialization;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>,
        IConsumer<ScheduleRecurringMessage>
    {
        readonly IScheduler _scheduler;

        public ScheduleMessageConsumer(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var correlationId = context.Message.CorrelationId.ToString("N");

            var jobKey = new JobKey(correlationId);

            var jobDetail = await CreateJobDetail(context, context.Message.Destination, jobKey).ConfigureAwait(false);

            var triggerKey = new TriggerKey(correlationId);
            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(context.Message.ScheduledTime)
                .WithSchedule(SimpleScheduleBuilder.Create().WithMisfireHandlingInstructionFireNow())
                .WithIdentity(triggerKey)
                .Build();

            if (await _scheduler.CheckExists(trigger.Key, context.CancellationToken).ConfigureAwait(false))
                await _scheduler.UnscheduleJob(trigger.Key, context.CancellationToken).ConfigureAwait(false);

            await _scheduler.ScheduleJob(jobDetail, trigger, context.CancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Scheduled: {Key} {Schedule}", jobKey, trigger.GetNextFireTimeUtc());
        }

        public async Task Consume(ConsumeContext<ScheduleRecurringMessage> context)
        {
            var jobKey = new JobKey(context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);

            var jobDetail = await CreateJobDetail(context, context.Message.Destination, jobKey).ConfigureAwait(false);

            var triggerKey = new TriggerKey("Recurring.Trigger." + context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);

            var trigger = CreateTrigger(context.Message.Schedule, jobDetail, triggerKey);

            if (await _scheduler.CheckExists(triggerKey, context.CancellationToken).ConfigureAwait(false))
                await _scheduler.UnscheduleJob(triggerKey, context.CancellationToken).ConfigureAwait(false);

            await _scheduler.ScheduleJob(jobDetail, trigger, context.CancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Scheduled: {Key} {Schedule}", jobKey, trigger.GetNextFireTimeUtc());
        }

        ITrigger CreateTrigger(RecurringSchedule schedule, IJobDetail jobDetail, TriggerKey triggerKey)
        {
            var tz = TimeZoneInfo.Local;
            if (!string.IsNullOrWhiteSpace(schedule.TimeZoneId) && schedule.TimeZoneId != tz.Id)
                tz = TimeZoneUtil.FindTimeZoneById(schedule.TimeZoneId);

            var triggerBuilder = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithIdentity(triggerKey)
                .StartAt(schedule.StartTime)
                .WithDescription(schedule.Description)
                .WithCronSchedule(schedule.CronExpression, x =>
                {
                    x.InTimeZone(tz);
                    switch (schedule.MisfirePolicy)
                    {
                        case MissedEventPolicy.Skip:
                            x.WithMisfireHandlingInstructionDoNothing();
                            break;

                        case MissedEventPolicy.Send:
                            x.WithMisfireHandlingInstructionFireAndProceed();
                            break;
                    }
                });

            if (schedule.EndTime.HasValue)
                triggerBuilder.EndAt(schedule.EndTime);

            return triggerBuilder.Build();
        }

        static async Task<IJobDetail> CreateJobDetail(ConsumeContext context, Uri destination, JobKey jobKey, Guid? tokenId = default)
        {
            string body = Encoding.UTF8.GetString(context.ReceiveContext.GetBody());

            var mediaType = context.ReceiveContext.ContentType?.MediaType;

            if (JsonMessageSerializer.JsonContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                body = TranslateJsonBody(body, destination.ToString());
            else if (XmlMessageSerializer.XmlContentType.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                body = TranslateXmlBody(body, destination.ToString());
            else
                throw new InvalidOperationException("Only JSON and XML messages can be scheduled");

            var builder = JobBuilder.Create<ScheduledMessageJob>()
                .RequestRecovery(true)
                .WithIdentity(jobKey)
                .UsingJobData("Destination", ToString(destination))
                .UsingJobData("ResponseAddress", ToString(context.ResponseAddress))
                .UsingJobData("FaultAddress", ToString(context.FaultAddress))
                .UsingJobData("Body", body)
                .UsingJobData("ContentType", mediaType);

            if (context.MessageId.HasValue)
                builder = builder.UsingJobData("MessageId", context.MessageId.Value.ToString());

            if (context.CorrelationId.HasValue)
                builder = builder.UsingJobData("CorrelationId", context.CorrelationId.Value.ToString());

            if (context.ConversationId.HasValue)
                builder = builder.UsingJobData("ConversationId", context.ConversationId.Value.ToString());

            if (context.InitiatorId.HasValue)
                builder = builder.UsingJobData("InitiatorId", context.InitiatorId.Value.ToString());

            if (context.RequestId.HasValue)
                builder = builder.UsingJobData("RequestId", context.RequestId.Value.ToString());

            if (context.ExpirationTime.HasValue)
                builder = builder.UsingJobData("ExpirationTime", context.ExpirationTime.Value.ToString("O"));

            if (tokenId.HasValue)
                builder = builder.UsingJobData("TokenId", tokenId.Value.ToString("N"));

            var headers = context.Headers.GetAll();
            if (headers.Any())
                builder = builder.UsingJobData("HeadersAsJson", JsonConvert.SerializeObject(headers));

            var jobDetail = builder
                .Build();

            return jobDetail;
        }

        static string ToString(Uri uri)
        {
            return uri?.ToString() ?? "";
        }

        static string TranslateJsonBody(string body, string destination)
        {
            var envelope = JObject.Parse(body);

            envelope["destinationAddress"] = destination;

            var message = envelope["message"];

            var payload = message["payload"];
            var payloadType = message["payloadType"];

            envelope["message"] = payload;
            envelope["messageType"] = payloadType;

            return JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        static string TranslateXmlBody(string body, string destination)
        {
            using (var reader = new StringReader(body))
            {
                var document = XDocument.Load(reader);

                var envelope = (from e in document.Descendants("envelope") select e).Single();

                var destinationAddress = (from a in envelope.Descendants("destinationAddress") select a).Single();

                var message = (from m in envelope.Descendants("message") select m).Single();
                IEnumerable<XElement> messageType = (from mt in envelope.Descendants("messageType") select mt);

                var payload = (from p in message.Descendants("payload") select p).Single();
                IEnumerable<XElement> payloadType = (from pt in message.Descendants("payloadType") select pt);

                message.Remove();
                messageType.Remove();

                destinationAddress.Value = destination;

                message = new XElement("message");
                message.Add(payload.Descendants());
                envelope.Add(message);

                envelope.Add(payloadType.Select(x => new XElement("messageType", x.Value)));

                return document.ToString(SaveOptions.DisableFormatting);
            }
        }
    }
}
