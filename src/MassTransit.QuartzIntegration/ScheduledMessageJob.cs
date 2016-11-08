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
    using System.Net.Mime;
    using GreenPipes;
    using Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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

                throw new JobExecutionException(ex, context.RefireCount < 5);
            }
        }

        IPipe<SendContext> CreateMessageContext(Uri sourceAddress, Uri destinationAddress, string triggerKey)
        {
            IPipe<SendContext> sendPipe = Pipe.New<SendContext>(x =>
            {
                x.UseExecute(context =>
                {
                    context.DestinationAddress = destinationAddress;
                    context.SourceAddress = sourceAddress;
                    context.ResponseAddress = ToUri(ResponseAddress);
                    context.FaultAddress = ToUri(FaultAddress);

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

                    var bodySerializer = new StringMessageSerializer(new ContentType(ContentType), Body);

                    if (!string.IsNullOrWhiteSpace(PayloadMessageHeadersAsJson))
                    {
                        var headers = JObject.Parse(PayloadMessageHeadersAsJson).ToObject<Dictionary<string, object>>();

                        if (string.Compare(ContentType, JsonMessageSerializer.JsonContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                            bodySerializer.UpdateJsonHeaders(headers);
                        else if (string.Compare(ContentType, XmlMessageSerializer.XmlContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                            bodySerializer.UpdateXmlHeaders(headers);
                    }

                    context.Serializer = bodySerializer;
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

        static Uri ToUri(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            return new Uri(s);
        }


        class Scheduled
        {
        }
    }
}