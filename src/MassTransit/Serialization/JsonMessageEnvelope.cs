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


    class JsonMessageEnvelope :
        MessageEnvelope
    {
        public JsonMessageEnvelope(SendContext context, object message, IEnumerable<string> messageTypeNames)
        {
            if (context.MessageId.HasValue)
                MessageId = context.MessageId.Value.ToString("N");

            if (context.RequestId.HasValue)
                RequestId = context.RequestId.Value.ToString("N");

            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.Value.ToString("N");

            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();

            if (context.DestinationAddress != null)
                DestinationAddress = context.DestinationAddress.ToString();

            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();

            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();

            MessageType = new List<string>(messageTypeNames);

            Message = message;

            if (context.TimeToLive.HasValue)
                ExpirationTime = DateTime.UtcNow + context.TimeToLive;

            Headers = new Dictionary<string, object>();

            foreach (var header in context.Headers)
                Headers[header.Key] = header.Value;
        }

        public string MessageId { get; private set; }
        public string RequestId { get; private set; }
        public string CorrelationId { get; private set; }
        public string SourceAddress { get; private set; }
        public string DestinationAddress { get; private set; }
        public string ResponseAddress { get; private set; }
        public string FaultAddress { get; private set; }
        public IList<string> MessageType { get; private set; }
        public object Message { get; private set; }
        public DateTime? ExpirationTime { get; private set; }
        public IDictionary<string, object> Headers { get; private set; }
    }
}