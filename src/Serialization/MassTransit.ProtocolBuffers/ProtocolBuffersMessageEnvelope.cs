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
namespace MassTransit.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using ProtoBuf;
    using Util;


    [ProtoContract]
    public class ProtocolBuffersMessageEnvelope
    {
        protected ProtocolBuffersMessageEnvelope()
        {
        }

        public ProtocolBuffersMessageEnvelope(SendContext context, string[] messageTypeNames)
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

            if (context.TimeToLive.HasValue)
                ExpirationTime = DateTime.UtcNow + context.TimeToLive;

            Headers = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                Headers[header.Key] = header.Value;

            Host = (BusHostInfo)HostMetadataCache.Host;
        }

        [ProtoMember(1)]
        public string MessageId { get; private set; }

        [ProtoMember(2)]
        public string RequestId { get; private set; }

        [ProtoMember(3)]
        public string CorrelationId { get; private set; }

        [ProtoMember(4)]
        public string ConversationId { get; private set; }

        [ProtoMember(5)]
        public string InitiatorId { get; private set; }

        [ProtoMember(6)]
        public string SourceAddress { get; private set; }

        [ProtoMember(7)]
        public string DestinationAddress { get; private set; }

        [ProtoMember(8)]
        public string ResponseAddress { get; private set; }

        [ProtoMember(9)]
        public string FaultAddress { get; private set; }

        [ProtoMember(10)]
        public string[] MessageType { get; private set; }

        [ProtoMember(11)]
        public DateTime? ExpirationTime { get; private set; }

        [ProtoMember(12)]
        public IDictionary<string, object> Headers { get; private set; }

        [ProtoMember(13)]
        public BusHostInfo Host { get; private set; }
    }
}