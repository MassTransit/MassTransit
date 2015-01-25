// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
        IConsumer<ScheduleMessage>
    {
        static readonly ILog _log = Logger.Get<ScheduleMessageConsumer>();
        readonly IScheduler _scheduler;

        public ScheduleMessageConsumer(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("ScheduleMessage: {0} at {1}", context.Message.CorrelationId,
                    context.Message.ScheduledTime);
            }

            string body;
            using (var ms = new MemoryStream())
            {
                await context.ReceiveContext.Body.CopyToAsync(ms);

                body = Encoding.UTF8.GetString(ms.ToArray());
            }

            if (string.Compare(context.ReceiveContext.ContentType.MediaType, JsonMessageSerializer.JsonContentType.MediaType,
                StringComparison.OrdinalIgnoreCase)
                == 0)
                body = TranslateJsonBody(body, context.Message.Destination.ToString());
            else if (string.Compare(context.ReceiveContext.ContentType.MediaType, XmlMessageSerializer.XmlContentType.MediaType,
                StringComparison.OrdinalIgnoreCase) == 0)
                body = TranslateXmlBody(body, context.Message.Destination.ToString());
            else
                throw new InvalidOperationException("Only JSON and XML messages can be scheduled");

            var builder = JobBuilder.Create<ScheduledMessageJob>()
                .RequestRecovery(true)
                .WithIdentity(context.Message.CorrelationId.ToString("N"))
                .UsingJobData("Destination", ToString(context.Message.Destination))
                .UsingJobData("ResponseAddress", ToString(context.ResponseAddress))
                .UsingJobData("FaultAddress", ToString(context.FaultAddress))
                .UsingJobData("Body", body)
                .UsingJobData("ContentType", context.ReceiveContext.ContentType.MediaType);

            if (context.MessageId.HasValue)
                builder = builder.UsingJobData("MessageId", context.MessageId.Value.ToString());
            if (context.CorrelationId.HasValue)
                builder = builder.UsingJobData("CorrelationId", context.CorrelationId.Value.ToString());
            if (context.RequestId.HasValue)
                builder = builder.UsingJobData("RequestId", context.RequestId.Value.ToString());
            if (context.ExpirationTime.HasValue)
                builder = builder.UsingJobData("ExpirationTime", context.ExpirationTime.Value.ToString());

            IJobDetail jobDetail = builder
                .UsingJobData("HeadersAsJson", JsonConvert.SerializeObject(context.Headers.Headers))
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(context.Message.ScheduledTime)
                .WithSchedule(SimpleScheduleBuilder.Create().WithMisfireHandlingInstructionFireNow())
                .WithIdentity(new TriggerKey(context.Message.CorrelationId.ToString("N")))
                .Build();

            _scheduler.ScheduleJob(jobDetail, trigger);
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