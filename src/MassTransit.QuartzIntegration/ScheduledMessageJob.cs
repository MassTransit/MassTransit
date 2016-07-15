// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Xml.Linq;
    using Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Pipeline;
    using Quartz;
    using Serialization;
    using Util;


    public class ScheduledMessageJob :
        IJob
    {
        static readonly ILog _log = Logger.Get<ScheduledMessageJob>();
        readonly IBus _bus;

        public ScheduledMessageJob(IBus bus)
        {
            _bus = bus;
        }

        public string Destination { get; set; }
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

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var destinationAddress = new Uri(Destination);
                var sourceAddress = _bus.Address;

                IPipe<SendContext> sendPipe = CreateMessageContext(sourceAddress, destinationAddress, context.Trigger.Key.Name);

                var endpoint = TaskUtil.Await(() => _bus.GetSendEndpoint(destinationAddress));

                var scheduled = new Scheduled();

                TaskUtil.Await(() => endpoint.Send(scheduled, sendPipe));
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.InvariantCulture,
                    "An exception occurred sending message {0} to {1}", MessageType, Destination);
                _log.Error(message, ex);

                throw new JobExecutionException(message, ex);
            }
        }

        IPipe<SendContext> CreateMessageContext(Uri sourceAddress, Uri destinationAddress, string triggerKey)
        {
            IPipe<SendContext> sendPipe = Pipe.New<SendContext>(x =>
            {
                x.UseExecute(context =>
                {
                    context.DestinationAddress = (destinationAddress);
                    context.SourceAddress = (sourceAddress);
                    context.ResponseAddress = (ToUri(ResponseAddress));
                    context.FaultAddress = (ToUri(FaultAddress));

                    SetHeaders(context);

                    context.MessageId = ConvertIdToGuid(MessageId);
                    context.RequestId = ConvertIdToGuid(RequestId);
                    context.CorrelationId = ConvertIdToGuid(CorrelationId);
                    context.ConversationId = ConvertIdToGuid(ConversationId);
                    context.InitiatorId = ConvertIdToGuid(InitiatorId);

                    Guid? tokenId = ConvertIdToGuid(TokenId);
                    if (tokenId.HasValue)
                    {
                        context.Headers.Set(MessageHeaders.SchedulingTokenId, tokenId.Value.ToString("N"));
                    }

                    context.Headers.Set(MessageHeaders.QuartzTriggerKey, triggerKey);

                    if (!string.IsNullOrEmpty(ExpirationTime))
                        context.TimeToLive = DateTime.UtcNow - DateTime.Parse(ExpirationTime);

                    if (string.Compare(ContentType, JsonMessageSerializer.JsonContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                        Body = UpdateJsonHeaders(Body);
                    else if (string.Compare(ContentType, XmlMessageSerializer.XmlContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                        Body = UpdateXmlHeaders(Body);

                    context.Serializer = new ScheduledBodySerializer(new ContentType(ContentType), Encoding.UTF8.GetBytes(Body));
                });
            });

            return sendPipe;
        }

        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default(Guid?);

            Guid messageId;
            if (Guid.TryParse(id, out messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        void SetHeaders(SendContext context)
        {
            if (string.IsNullOrEmpty(HeadersAsJson))
                return;

            var headers = JsonConvert.DeserializeObject<IDictionary<string, object>>(HeadersAsJson);
            foreach (KeyValuePair<string, object> header in headers)
                context.Headers.Set(header.Key, header.Value);
        }

        string UpdateJsonHeaders(string body)
        {
            if (string.IsNullOrEmpty(PayloadMessageHeadersAsJson))
                return body;

            var envelope = JObject.Parse(body);

            var payloadHeaders = JObject.Parse(PayloadMessageHeadersAsJson).ToObject<Dictionary<string, object>>();

            var headersToken = envelope["headers"] ?? new JObject();
            var headers = headersToken.ToObject<Dictionary<string, object>>();

            foreach (KeyValuePair<string, object> payloadHeader in payloadHeaders)
            {
                headers[payloadHeader.Key] = payloadHeader.Value;
            }
            envelope["headers"] = JToken.FromObject(headers);

            return JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        string UpdateXmlHeaders(string body)
        {
            if (string.IsNullOrEmpty(PayloadMessageHeadersAsJson))
                return body;

            using (var reader = new StringReader(body))
            {
                var document = XDocument.Load(reader);

                var envelope = (from e in document.Descendants("envelope") select e).Single();

                var headers = (from h in envelope.Descendants("headers") select h).SingleOrDefault();
                if (headers == null)
                {
                    headers = new XElement("headers");
                    envelope.Add(headers);
                }

                var payloadHeaders = JObject.Parse(PayloadMessageHeadersAsJson).ToObject<Dictionary<string, object>>();

                foreach (KeyValuePair<string, object> payloadHeader in payloadHeaders)
                {
                    headers.Add(new XElement(payloadHeader.Key, payloadHeader.Value));
                }

                return document.ToString();
            }
        }

        static Uri ToUri(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            return new Uri(s);
        }


        class Scheduled
        {
        }


        class ScheduledBodySerializer :
            IMessageSerializer
        {
            readonly byte[] _body;

            public ScheduledBodySerializer(ContentType contentType, byte[] body)
            {
                ContentType = contentType;
                _body = body;
            }

            public ContentType ContentType { get; }

            public void Serialize<T>(Stream stream, SendContext<T> context)
                where T : class
            {
                stream.Write(_body, 0, _body.Length);
            }
        }
    }
}