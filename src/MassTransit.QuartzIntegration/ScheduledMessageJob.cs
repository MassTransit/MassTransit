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
    using System.Globalization;
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using Logging;
    using Newtonsoft.Json;
    using Pipeline;
    using Quartz;
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

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var destinationAddress = new Uri(Destination);
                Uri sourceAddress = _bus.Address;

                IPipe<SendContext> sendPipe = CreateMessageContext(sourceAddress, destinationAddress, context.Trigger.Key.Name);

                ISendEndpoint endpoint = TaskUtil.Await(() => _bus.GetSendEndpoint(destinationAddress));

                var scheduled = new Scheduled();

                TaskUtil.Await(() => endpoint.Send(scheduled, sendPipe));
            }
            catch (Exception ex)
            {
                string message = string.Format(CultureInfo.InvariantCulture,
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

                    context.ContentType = new ContentType(ContentType);

                    context.Serializer = new ScheduledBodySerializer(context.ContentType, Encoding.UTF8.GetBytes(Body));
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
            foreach (var header in headers)
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