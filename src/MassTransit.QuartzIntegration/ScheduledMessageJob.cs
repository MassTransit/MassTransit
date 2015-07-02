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
        public string HeadersAsJson { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var destinationAddress = new Uri(Destination);
                Uri sourceAddress = _bus.Address;

                IPipe<SendContext> sendPipe = CreateMessageContext(sourceAddress, destinationAddress);

                ISendEndpoint endpoint = _bus.GetSendEndpoint(destinationAddress)
                    .Result;

                var scheduled = new Scheduled();
                endpoint.Send(scheduled, sendPipe)
                    .Wait();
            }
            catch (Exception ex)
            {
                string message = string.Format(CultureInfo.InvariantCulture,
                    "An exception occurred sending message {0} to {1}", MessageType, Destination);
                _log.Error(message, ex);

                throw new JobExecutionException(message, ex);
            }
        }

        IPipe<SendContext> CreateMessageContext(Uri sourceAddress, Uri destinationAddress)
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

                    if (!string.IsNullOrEmpty(MessageId))
                        context.MessageId = new Guid(MessageId);
                    if (!string.IsNullOrEmpty(RequestId))
                        context.RequestId = new Guid(RequestId);
                    if (!string.IsNullOrEmpty(CorrelationId))
                        context.CorrelationId = new Guid(CorrelationId);

                    if (!string.IsNullOrEmpty(ExpirationTime))
                        context.TimeToLive = DateTime.UtcNow - DateTime.Parse(ExpirationTime);

                    context.ContentType = new ContentType(ContentType);

                    context.Serializer = new ScheduledBodySerializer(context.ContentType, Encoding.UTF8.GetBytes(Body));
                });
            });

            return sendPipe;
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
            readonly ContentType _contentType;

            public ScheduledBodySerializer(ContentType contentType, byte[] body)
            {
                _contentType = contentType;
                _body = body;
            }

            public ContentType ContentType
            {
                get { return _contentType; }
            }

            public void Serialize<T>(Stream stream, SendContext<T> context)
                where T : class
            {
                stream.Write(_body, 0, _body.Length);
            }
        }
    }
}