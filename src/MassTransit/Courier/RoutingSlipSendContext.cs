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
namespace MassTransit.Courier
{
    using System;
    using System.Threading;
    using Context;
    using MassTransit.Serialization;
    using Util;


    public class RoutingSlipSendContext<T> :
        BaseSendContext<T>
        where T : class
    {
        public RoutingSlipSendContext(T message, CancellationToken cancellationToken, Uri destinationAddress)
            : base(message, cancellationToken)
        {
            DestinationAddress = destinationAddress;

            Serializer = new JsonMessageSerializer();
        }

        public MessageEnvelope GetMessageEnvelope()
        {
            var envelope = new JsonMessageEnvelope(this, Message, TypeMetadataCache<T>.MessageTypeNames);

            return envelope;
//            var message = new JsonSerializedMessage();
//
//            if (CorrelationId.HasValue)
//                message.CorrelationId = CorrelationId.Value.ToString();
//            if (ConversationId.HasValue)
//                message.CorrelationId = ConversationId.Value.ToString();
//            if (InitiatorId.HasValue)
//                message.CorrelationId = InitiatorId.Value.ToString();
//            if (RequestId.HasValue)
//                message.CorrelationId = RequestId.Value.ToString();
//            if (TimeToLive.HasValue)
//                message.ExpirationTime = JsonConvert.ToString(DateTime.UtcNow + TimeToLive);
//
//            byte[] body = Body;
//
//            message.Body = Encoding.UTF8.GetString(body);
//
//            return message;
        }
    }
}