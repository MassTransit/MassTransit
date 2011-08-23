// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    public static class EnvelopeExtensions
    {
        public static void SetUsingEnvelope(this IReceiveContext context, Envelope envelope)
        {
            context.SetRequestId(envelope.RequestId);
            context.SetConversationId(envelope.ConversationId);
            context.SetCorrelationId(envelope.CorrelationId);
            context.SetSourceAddress(envelope.SourceAddress.ToUriOrNull());
            context.SetDestinationAddress(envelope.DestinationAddress.ToUriOrNull());
            context.SetResponseAddress(envelope.ResponseAddress.ToUriOrNull());
            context.SetFaultAddress(envelope.FaultAddress.ToUriOrNull());
            context.SetNetwork(envelope.Network);
            context.SetRetryCount(envelope.RetryCount);
            if (envelope.ExpirationTime.HasValue)
                context.SetExpirationTime(envelope.ExpirationTime.Value);

            foreach (var header in envelope.Headers)
            {
                context.SetHeader(header.Key, header.Value);
            }
        }

        public static void SetUsingContext(this Envelope envelope, ISendContext context)
        {
            envelope.RequestId = context.RequestId;
            envelope.ConversationId = context.ConversationId;
            envelope.CorrelationId = context.CorrelationId;
            envelope.SourceAddress = context.SourceAddress.ToStringOrNull() ?? envelope.SourceAddress;
            envelope.DestinationAddress = context.DestinationAddress.ToStringOrNull() ?? envelope.DestinationAddress;
            envelope.ResponseAddress = context.ResponseAddress.ToStringOrNull() ?? envelope.ResponseAddress;
            envelope.FaultAddress = context.FaultAddress.ToStringOrNull() ?? envelope.FaultAddress;
            envelope.Network = context.Network;
            envelope.RetryCount = context.RetryCount;
            if (context.ExpirationTime.HasValue)
                envelope.ExpirationTime = context.ExpirationTime.Value;

            foreach (var header in context.Headers)
            {
                envelope.Headers[header.Key] = header.Value;
            }
        }
    }
}