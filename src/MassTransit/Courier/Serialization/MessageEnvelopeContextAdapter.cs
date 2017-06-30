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

namespace MassTransit.Courier.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Serialization;


    public class MessageEnvelopeContextAdapter :
        IPipe<SendContext>
    {
        readonly string _contentType;
        readonly object _subscriptionMessage;
        readonly MessageEnvelope _envelope;
        readonly IPipe<SendContext> _pipe;

        public MessageEnvelopeContextAdapter(IPipe<SendContext> pipe, MessageEnvelope envelope, string contentType, object subscriptionMessage)
        {
            _pipe = pipe;
            _envelope = envelope;
            _contentType = contentType;
            _subscriptionMessage = subscriptionMessage;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext context)
        {
            context.DestinationAddress = ToUri(_envelope.DestinationAddress);

            context.ResponseAddress = ToUri(_envelope.ResponseAddress);
            context.FaultAddress = ToUri(_envelope.FaultAddress);

            SetHeaders(context);

            context.RequestId = ConvertIdToGuid(_envelope.RequestId);
            context.CorrelationId = ConvertIdToGuid(_envelope.CorrelationId);

            if (_envelope.ExpirationTime.HasValue)
                context.TimeToLive = _envelope.ExpirationTime.Value - DateTime.UtcNow;

            if (_pipe != null)
                await _pipe.Send(context).ConfigureAwait(false);

            var bodySerializer = new EnvelopeMessageSerializer(new ContentType(_contentType), _envelope, _subscriptionMessage);

            context.Serializer = bodySerializer;
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
            foreach (KeyValuePair<string, object> header in _envelope.Headers)
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