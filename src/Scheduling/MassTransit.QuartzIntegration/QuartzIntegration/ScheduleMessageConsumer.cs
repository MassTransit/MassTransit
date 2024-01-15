namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
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
        const string ScheduleMessageJobId = "MassTransitScheduleMessageJob";

        readonly ISchedulerFactory _schedulerFactory;
        JobKey? _jobKey;

        public ScheduleMessageConsumer(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var jobKey = await EnsureJobExists(context.CancellationToken).ConfigureAwait(false);

            var messageBody = context.SerializerContext.GetMessageSerializer(context.Message.Payload, context.Message.PayloadType)
                .GetMessageBody(new MessageSendContext<ScheduleMessage>(context.Message));

            var correlationId = context.Message.CorrelationId.ToString("N");
            var triggerKey = new TriggerKey(correlationId);

            var builder = TriggerBuilder.Create()
                .ForJob(jobKey)
                .StartAt(context.Message.ScheduledTime)
                .WithSchedule(SimpleScheduleBuilder.Create().WithMisfireHandlingInstructionFireNow())
                .WithIdentity(triggerKey);

            var trigger = PopulateTrigger(context, builder, messageBody, context.Message.Destination, context.Message.PayloadType, messageId: context.MessageId,
                tokenId: context.Message.CorrelationId);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            if (await scheduler.CheckExists(trigger.Key, context.CancellationToken).ConfigureAwait(false))
                await scheduler.UnscheduleJob(trigger.Key, context.CancellationToken).ConfigureAwait(false);

            await scheduler.ScheduleJob(trigger, context.CancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Scheduled: {Key} {Schedule}", trigger.Key, trigger.GetNextFireTimeUtc());
        }

        public async Task Consume(ConsumeContext<ScheduleRecurringMessage> context)
        {
            var jobKey = await EnsureJobExists(context.CancellationToken).ConfigureAwait(false);

            var messageBody = context.SerializerContext.GetMessageSerializer(context.Message.Payload, context.Message.PayloadType)
                .GetMessageBody(new MessageSendContext<ScheduleRecurringMessage>(context.Message));

            var schedule = context.Message.Schedule;
            var triggerKey = new TriggerKey(QuartzConstants.RecurringTriggerPrefix + schedule.ScheduleId, schedule.ScheduleGroup);

            var tz = TimeZoneInfo.Local;
            if (!string.IsNullOrWhiteSpace(schedule.TimeZoneId) && schedule.TimeZoneId != tz.Id)
                tz = TimeZoneUtil.FindTimeZoneById(schedule.TimeZoneId);

            var triggerBuilder = TriggerBuilder.Create()
                .ForJob(jobKey)
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

            var trigger = PopulateTrigger(context, triggerBuilder, messageBody, context.Message.Destination, context.Message.PayloadType,
                messageId: default, tokenId: context.Message.CorrelationId);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            if (await scheduler.CheckExists(triggerKey, context.CancellationToken).ConfigureAwait(false))
                await scheduler.UnscheduleJob(triggerKey, context.CancellationToken).ConfigureAwait(false);

            await scheduler.ScheduleJob(trigger, context.CancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Scheduled: {Key} {Schedule}", triggerKey, trigger.GetNextFireTimeUtc());
        }

        static ITrigger PopulateTrigger(ConsumeContext context, TriggerBuilder builder, MessageBody messageBody, Uri destination,
            string[] messageTypes, Guid? messageId = default, Guid? tokenId = default)
        {
            builder = builder
                .UsingJobData("Destination", ToString(destination))
                .UsingJobData("Body", messageBody.GetString())
                .UsingJobData("ContentType", context.ReceiveContext.ContentType.ToString())
                .UsingJobData("MessageType", string.Join(";", messageTypes));

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

            if (context.ReceiveContext.TryGetPayload<TransportReceiveContext>(out var transportReceiveContext))
            {
                IDictionary<string, object>? properties = transportReceiveContext.GetTransportProperties();
                if (properties != null)
                    builder = builder.UsingJobData("TransportProperties", JsonSerializer.Serialize(properties, SystemTextJsonMessageSerializer.Options));
            }

            var trigger = builder
                .Build();

            return trigger;
        }

        async Task<JobKey> EnsureJobExists(CancellationToken cancellationToken)
        {
            var jobKey = Volatile.Read(ref _jobKey);
            if (jobKey != null)
                return jobKey;

            jobKey = new JobKey(ScheduleMessageJobId);

            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);

            if (await scheduler.CheckExists(jobKey, cancellationToken).ConfigureAwait(false))
                return jobKey;

            var jobDetail = JobBuilder.Create<ScheduledMessageJob>()
                .RequestRecovery()
                .StoreDurably()
                .WithIdentity(jobKey)
                .WithDescription("MassTransit Scheduled Message Job")
                .Build();

            await scheduler.AddJob(jobDetail, true, cancellationToken).ConfigureAwait(false);

            Interlocked.CompareExchange(ref _jobKey, jobKey, null);

            return jobKey;
        }

        static string ToString(Uri? uri)
        {
            return uri?.ToString() ?? "";
        }
    }
}
