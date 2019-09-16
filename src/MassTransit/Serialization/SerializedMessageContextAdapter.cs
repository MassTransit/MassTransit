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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class SerializedMessageContextAdapter :
        IPipe<SendContext>
    {
        readonly SerializedMessage _message;
        readonly IPipe<SendContext> _pipe;
        readonly Uri _sourceAddress;

        public SerializedMessageContextAdapter(IPipe<SendContext> pipe, SerializedMessage message, Uri sourceAddress)
        {
            _pipe = pipe;
            _message = message;
            _sourceAddress = sourceAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext context)
        {
            context.DestinationAddress = _message.Destination;
            context.SourceAddress = _sourceAddress;

            context.ResponseAddress = ToUri(_message.ResponseAddress);
            context.FaultAddress = ToUri(_message.FaultAddress);

            SetHeaders(context);

            context.MessageId = ConvertIdToGuid(_message.MessageId);
            context.RequestId = ConvertIdToGuid(_message.RequestId);
            context.CorrelationId = ConvertIdToGuid(_message.CorrelationId);
            context.ConversationId = ConvertIdToGuid(_message.ConversationId);
            context.InitiatorId = ConvertIdToGuid(_message.InitiatorId);

            if (!string.IsNullOrEmpty(_message.ExpirationTime))
                context.TimeToLive = DateTime.UtcNow - DateTime.Parse(_message.ExpirationTime);

            var bodySerializer = new StringMessageSerializer(new ContentType(_message.ContentType), _message.Body);

            if (!string.IsNullOrWhiteSpace(_message.PayloadMessageHeadersAsJson))
            {
                var headers = JObject.Parse(_message.PayloadMessageHeadersAsJson).ToObject<Dictionary<string, object>>();

                if (string.Compare(_message.ContentType, JsonMessageSerializer.JsonContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                    bodySerializer.UpdateJsonHeaders(headers);
                else if (string.Compare(_message.ContentType, XmlMessageSerializer.XmlContentType.MediaType, StringComparison.OrdinalIgnoreCase) == 0)
                    bodySerializer.UpdateXmlHeaders(headers);
            }

            context.Serializer = bodySerializer;

            if (_pipe != null)
                await _pipe.Send(context).ConfigureAwait(false);
        }

        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        void SetHeaders(SendContext context)
        {
            if (string.IsNullOrEmpty(_message.HeadersAsJson))
                return;

            var headers = JsonConvert.DeserializeObject<IDictionary<string, object>>(_message.HeadersAsJson);
            foreach (KeyValuePair<string, object> header in headers)
                context.Headers.Set(header.Key, header.Value);
        }

        static Uri ToUri(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            return new Uri(s);
        }
    }
}
