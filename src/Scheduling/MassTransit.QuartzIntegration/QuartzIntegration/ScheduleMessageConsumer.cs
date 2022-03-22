namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Context;
    using Quartz;
    using Quartz.Util;
    using Scheduling;
    using Serialization;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>,
        IConsumer<ScheduleRecurringMessage>
    {
        readonly ISchedulerFactory _schedulerFactory;

        public ScheduleMessageConsumer(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var correlationId = context.Message.CorrelationId.ToString("N");

            var jobKey = new JobKey(correlationId);

            var messageBody = context.SerializerContext.GetMessageSerializer(context.Message.Payload, context.Message.PayloadType)
                .GetMessageBody(new MessageSendContext<ScheduleMessage>(context.Message));

            var jobDetail = await CreateJobDetail(context, context.Message.Destination, jobKey, messageBody, context.MessageId, context.Message.CorrelationId)
                .ConfigureAwait(false);

            var triggerKey = new TriggerKey(correlationId);
            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(context.Message.ScheduledTime)
                .WithSchedule(SimpleScheduleBuilder.Create().WithMisfireHandlingInstructionFireNow())
                .WithIdentity(triggerKey)
                .Build();

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            if (await scheduler.CheckExists(trigger.Key, context.CancellationToken).ConfigureAwait(false))
                await scheduler.UnscheduleJob(trigger.Key, context.CancellationToken).ConfigureAwait(false);

            await scheduler.ScheduleJob(jobDetail, trigger, context.CancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Scheduled: {Key} {Schedule}", jobKey, trigger.GetNextFireTimeUtc());
        }

        public async Task Consume(ConsumeContext<ScheduleRecurringMessage> context)
        {
            var jobKey = new JobKey(context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);

            var messageBody = context.SerializerContext.GetMessageSerializer(context.Message.Payload, context.Message.PayloadType)
                .GetMessageBody(new MessageSendContext<ScheduleRecurringMessage>(context.Message));

            var jobDetail = await CreateJobDetail(context, context.Message.Destination, jobKey, messageBody).ConfigureAwait(false);

            var triggerKey = new TriggerKey("Recurring.Trigger." + context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);

            var trigger = CreateTrigger(context.Message.Schedule, jobDetail, triggerKey);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            if (await scheduler.CheckExists(triggerKey, context.CancellationToken).ConfigureAwait(false))
                await scheduler.UnscheduleJob(triggerKey, context.CancellationToken).ConfigureAwait(false);

            await scheduler.ScheduleJob(jobDetail, trigger, context.CancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Scheduled: {Key} {Schedule}", jobKey, trigger.GetNextFireTimeUtc());
        }

        static ITrigger CreateTrigger(RecurringSchedule schedule, IJobDetail jobDetail, TriggerKey triggerKey)
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

        static async Task<IJobDetail> CreateJobDetail(ConsumeContext context, Uri destination, JobKey jobKey, MessageBody messageBody,
            Guid? messageId = default, Guid? tokenId = default)
        {
            var body = messageBody.GetString();

            var contentType = context.ReceiveContext.ContentType.ToString();

            var builder = JobBuilder.Create<ScheduledMessageJob>()
                .RequestRecovery()
                .WithIdentity(jobKey)
                .UsingJobData("Destination", ToString(destination))
                .UsingJobData("Body", body)
                .UsingJobData("ContentType", contentType);

            if (messageId.HasValue)
                builder = builder.UsingJobData("MessageId", messageId.Value.ToString());

            if (context.CorrelationId.HasValue)
                builder = builder.UsingJobData("CorrelationId", context.CorrelationId.Value.ToString());

            if (context.ConversationId.HasValue)
                builder = builder.UsingJobData("ConversationId", context.ConversationId.Value.ToString());

            if (context.InitiatorId.HasValue)
                builder = builder.UsingJobData("InitiatorId", context.InitiatorId.Value.ToString());

            if (context.RequestId.HasValue)
                builder = builder.UsingJobData("RequestId", context.RequestId.Value.ToString());

            if (context.SourceAddress != null)
                builder = builder.UsingJobData("SourceAddress", context.SourceAddress.ToString());

            if (context.ResponseAddress != null)
                builder = builder.UsingJobData("ResponseAddress", context.ResponseAddress.ToString());

            if (context.FaultAddress != null)
                builder = builder.UsingJobData("FaultAddress", context.FaultAddress.ToString());

            if (context.ExpirationTime.HasValue)
                builder = builder.UsingJobData("ExpirationTime", context.ExpirationTime.Value.ToString("O"));

            if (tokenId.HasValue)
                builder = builder.UsingJobData("TokenId", tokenId.Value.ToString("N"));

            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll().ToList();
            if (headers.Any())
                builder = builder.UsingJobData("HeadersAsJson", JsonSerializer.Serialize(headers, SystemTextJsonMessageSerializer.Options));

            var jobDetail = builder
                .Build();

            return jobDetail;
        }

        static string ToString(Uri? uri)
        {
            return uri?.ToString() ?? "";
        }
    }
}
