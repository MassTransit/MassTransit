// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Quartz;
    using Scheduling;
    using Serialization;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>,
        IConsumer<ScheduleRecurringMessage>
    {
        static readonly ILog _log = Logger.Get<ScheduleMessageConsumer>();
        readonly IScheduler _scheduler;

        public ScheduleMessageConsumer(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var correlationId = context.Message.CorrelationId.ToString("N");

            var jobKey = new JobKey(correlationId);
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ScheduleMessage: {0} at {1}", jobKey, context.Message.ScheduledTime);

            IJobDetail jobDetail = await CreateJobDetail(context, context.Message.Destination, jobKey).ConfigureAwait(false);

            ITrigger trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(context.Message.ScheduledTime)
                .WithSchedule(SimpleScheduleBuilder.Create().WithMisfireHandlingInstructionFireNow())
                .WithIdentity(new TriggerKey(correlationId))
                .Build();

            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task Consume(ConsumeContext<ScheduleRecurringMessage> context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ScheduleRecurringMessage: {0} at {1}", context.Message.CorrelationId, context.Message.Schedule.ScheduleId);

            var jobKey = new JobKey(context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);

            IJobDetail jobDetail = await CreateJobDetail(context, context.Message.Destination, jobKey).ConfigureAwait(false);

            var triggerKey = new TriggerKey("Recurring.Trigger." + context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);

            ITrigger trigger = CreateTrigger(context.Message.Schedule, jobDetail, triggerKey);

            if (_scheduler.CheckExists(triggerKey))
                _scheduler.RescheduleJob(triggerKey, trigger);
            else
                _scheduler.ScheduleJob(jobDetail, trigger);
        }

        ITrigger CreateTrigger(RecurringSchedule message, IJobDetail jobDetail, TriggerKey triggerKey)
        {
            TimeZoneInfo tz = TimeZoneInfo.Local;
            if (!string.IsNullOrWhiteSpace(message.TimeZoneId) && message.TimeZoneId != tz.Id)
                tz = TimeZoneInfo.FindSystemTimeZoneById(message.TimeZoneId);

            TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithIdentity(triggerKey)
                .StartAt(message.StartTime)
                .WithCronSchedule(message.CronExpression, x =>
                {
                    x.InTimeZone(tz);
                    switch (message.MisfirePolicy)
                    {
                        case MissedEventPolicy.Skip:
                            x.WithMisfireHandlingInstructionDoNothing();
                            break;
                        case MissedEventPolicy.Send:
                            x.WithMisfireHandlingInstructionFireAndProceed();
                            break;
                    }
                });

            if (message.EndTime.HasValue)
                triggerBuilder.EndAt(message.EndTime);

            return triggerBuilder.Build();
        }

        static async Task<IJobDetail> CreateJobDetail(ConsumeContext context, Uri destination, JobKey jobKey, Guid? tokenId = default(Guid?))
        {
            string body;
            using (var ms = new MemoryStream())
            {
                using (Stream bodyStream = context.ReceiveContext.GetBody())
                {
                    await bodyStream.CopyToAsync(ms).ConfigureAwait(false);
                }

                body = Encoding.UTF8.GetString(ms.ToArray());
            }

            if (string.Compare(context.ReceiveContext.ContentType.MediaType, JsonMessageSerializer.JsonContentType.MediaType,
                StringComparison.OrdinalIgnoreCase)
                == 0)
                body = TranslateJsonBody(body, destination.ToString());
            else if (string.Compare(context.ReceiveContext.ContentType.MediaType, XmlMessageSerializer.XmlContentType.MediaType,
                StringComparison.OrdinalIgnoreCase) == 0)
                body = TranslateXmlBody(body, destination.ToString());
            else
                throw new InvalidOperationException("Only JSON and XML messages can be scheduled");

            JobBuilder builder = JobBuilder.Create<ScheduledMessageJob>()
                .RequestRecovery(true)
                .WithIdentity(jobKey)
                .UsingJobData("Destination", ToString(destination))
                .UsingJobData("ResponseAddress", ToString(context.ResponseAddress))
                .UsingJobData("FaultAddress", ToString(context.FaultAddress))
                .UsingJobData("Body", body)
                .UsingJobData("ContentType", context.ReceiveContext.ContentType.MediaType);

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
                builder = builder.UsingJobData("ExpirationTime", context.ExpirationTime.Value.ToString());

            if (tokenId.HasValue)
                builder = builder.UsingJobData("TokenId", tokenId.Value.ToString("N"));

            IJobDetail jobDetail = builder
                .UsingJobData("HeadersAsJson", JsonConvert.SerializeObject(context.Headers.GetAll()))
                .Build();

            return jobDetail;
        }

        static string ToString(Uri uri)
        {
            if (uri == null)
                return "";

            return uri.ToString();
        }

        static string TranslateJsonBody(string body, string destination)
        {
            JObject envelope = JObject.Parse(body);

            envelope["destinationAddress"] = destination;

            JToken message = envelope["message"];

            JToken payload = message["payload"];
            JToken payloadType = message["payloadType"];

            envelope["message"] = payload;
            envelope["messageType"] = payloadType;

            return JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        static string TranslateXmlBody(string body, string destination)
        {
            using (var reader = new StringReader(body))
            {
                XDocument document = XDocument.Load(reader);

                XElement envelope = (from e in document.Descendants("envelope") select e).Single();

                XElement destinationAddress = (from a in envelope.Descendants("destinationAddress") select a).Single();

                XElement message = (from m in envelope.Descendants("message") select m).Single();
                IEnumerable<XElement> messageType = (from mt in envelope.Descendants("messageType") select mt);

                XElement payload = (from p in message.Descendants("payload") select p).Single();
                IEnumerable<XElement> payloadType = (from pt in message.Descendants("payloadType") select pt);

                message.Remove();
                messageType.Remove();

                destinationAddress.Value = destination;

                message = new XElement("message");
                message.Add(payload.Descendants());
                envelope.Add(message);

                envelope.Add(payloadType.Select(x => new XElement("messageType", x.Value)));

                return document.ToString();
            }
        }
    }
}