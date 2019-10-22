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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using Metadata;
    using Util;


    public class JsonMessageEnvelope :
        MessageEnvelope
    {
        public JsonMessageEnvelope(SendContext context, object message, string[] messageTypeNames)
        {
            if (context.MessageId.HasValue)
                MessageId = context.MessageId.Value.ToString();

            if (context.RequestId.HasValue)
                RequestId = context.RequestId.Value.ToString();

            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.Value.ToString();

            if (context.ConversationId.HasValue)
                ConversationId = context.ConversationId.Value.ToString();

            if (context.InitiatorId.HasValue)
                InitiatorId = context.InitiatorId.Value.ToString();

            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();

            if (context.DestinationAddress != null)
                DestinationAddress = context.DestinationAddress.ToString();

            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();

            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();

            MessageType = messageTypeNames;

            Message = message;

            if (context.TimeToLive.HasValue)
                ExpirationTime = DateTime.UtcNow + context.TimeToLive;

            SentTime = DateTime.UtcNow;

            Headers = new Dictionary<string, object>();

            foreach (var header in context.Headers.GetAll())
                Headers[header.Key] = header.Value;

            Host = HostMetadataCache.Host;
        }

        public string MessageId { get; }
        public string RequestId { get; }
        public string CorrelationId { get; }
        public string ConversationId { get; }
        public string InitiatorId { get; }
        public string SourceAddress { get; }
        public string DestinationAddress { get; }
        public string ResponseAddress { get; }
        public string FaultAddress { get; }
        public string[] MessageType { get; }
        public object Message { get; }
        public DateTime? ExpirationTime { get; }
        public DateTime? SentTime { get; }
        public IDictionary<string, object> Headers { get; }
        public HostInfo Host { get; }
    }
}